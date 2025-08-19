using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.ObjectMapping;

namespace Abp.ApiKeyManagement.ApiKeys;

public class DefaultApiKeyStore(IApiKeyRepository apiKeyRepository, IDistributedCache<ApiKeyCacheItem, string> apikeyCache, IObjectMapper objectMapper) : IApiKeyStore, ITransientDependency
{
    public async Task<ApiKeyInfo?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var apiKey = await apikeyCache.GetOrAddAsync(prefix, async () =>
        {
            var apiKey = await apiKeyRepository.FindByPrefixAsync(prefix, cancellationToken);
            if (apiKey is null)
            {
                return ApiKeyCacheItem.NotFound;
            }
            var apiKeyInfo = objectMapper.Map<ApiKey, ApiKeyInfo>(apiKey);
            return new ApiKeyCacheItem
            {
                IsFound = true,
                ApiKey = apiKeyInfo
            };
        }, token: cancellationToken);
        
        return apiKey?.IsFound == true 
            ? apiKey.ApiKey 
            : null;
    }

    public async Task<ApiKeyInfo?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = id.ToString("N");
        var apiKey = await apikeyCache.GetOrAddAsync(key, async () =>
        {
            var apiKey = await apiKeyRepository.FindAsync(id, cancellationToken: cancellationToken);
            if (apiKey is null)
            {
                return ApiKeyCacheItem.NotFound;
            }
            var apiKeyInfo = objectMapper.Map<ApiKey, ApiKeyInfo>(apiKey);
            return new ApiKeyCacheItem
            {
                IsFound = true,
                ApiKey = apiKeyInfo
            };
        }, token: cancellationToken);
        
        return apiKey?.IsFound == true 
            ? apiKey.ApiKey 
            : null;
    }

    public class ApiKeyLocalEventHandler(IDistributedCache<ApiKeyCacheItem, string> apiKeyCache) : ILocalEventHandler<EntityChangedEventData<ApiKey>>, ITransientDependency
    {
        public async Task HandleEventAsync(EntityChangedEventData<ApiKey> eventData)
        {
            await apiKeyCache.RemoveAsync(eventData.Entity.Prefix);
            await apiKeyCache.RemoveAsync(eventData.Entity.Id.ToString("N"));
        }
    }
}