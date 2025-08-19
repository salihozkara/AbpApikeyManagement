using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Security.Claims;
using Volo.Abp.Timing;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyPrincipalProvider(
    ApiKeyManager apiKeyManager,
    IApiKeyHasher apiKeyHasher,
    IApiKeyStore apiKeyStore,
    IClock clock,
    IDistributedCache<ApiKeyPrincipalProvider.ApiKeyCacheItem> apikeyCache)
    : IApiKeyPrincipalProvider, ITransientDependency
{
    public async Task<ClaimsPrincipal?> GetApiKeyPrincipalOrNullAsync(string key)
    {
        if (key.IsNullOrWhiteSpace())
        {
            return null;
        }

        var prefix = apiKeyManager.GetPrefix(key);
        if (prefix.IsNullOrWhiteSpace())
        {
            return null; // Invalid key format
        }

        var rawKey = apiKeyManager.GetRawKey(key);
        if (rawKey.IsNullOrWhiteSpace())
        {
            return null; // Invalid key format
        }

        var apiKeyInfo = await apiKeyStore.FindByPrefixAsync(prefix);

        if (apiKeyInfo == null || apiKeyInfo.IsActive == false ||
            (apiKeyInfo.ExpirationTime.HasValue && apiKeyInfo.ExpirationTime.Value < clock.Now))
        {
            return null;
        }

        var sha256 = HashApiKey(key);
        var cache = await apikeyCache.GetOrAddAsync(sha256,
            () => Task.FromResult(new ApiKeyCacheItem { IsValid = apiKeyHasher.Verify(rawKey, apiKeyInfo.Hash) }), () =>
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // Cache for 30 minutes
                    SlidingExpiration = TimeSpan.FromMinutes(5) // Sliding expiration of 5 minutes
                });

        if (cache?.IsValid != true)
        {
            return null; // Invalid API key
        }

        var claims = new[]
        {
            new Claim(ApiKeyClaimTypes.ApiKeyId, apiKeyInfo.Id.ToString()),
            new Claim(AbpClaimTypes.UserId, apiKeyInfo.UserId.ToString()),
            new Claim(AbpClaimTypes.TenantId, apiKeyInfo.TenantId?.ToString() ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, ApiKeyAuthorizationConsts.AuthenticationType);

        return new ClaimsPrincipal(identity);
    }

    protected virtual string HashApiKey(string rawApiKey)
    {
        var bytes = Encoding.UTF8.GetBytes(rawApiKey);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    public class ApiKeyCacheItem
    {
        public bool IsValid { get; set; }
    }
    
    public class ApiKeyLocalEventHandler(IDistributedCache<ApiKeyCacheItem> apiKeyCache) : ILocalEventHandler<EntityChangedEventData<ApiKey>>, ITransientDependency
    {
        public async Task HandleEventAsync(EntityChangedEventData<ApiKey> eventData)
        {
            await apiKeyCache.RemoveAsync(eventData.Entity.Prefix);
        }
    }
}