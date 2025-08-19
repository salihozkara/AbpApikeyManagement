using System;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Services;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyManager(IApiKeyHasher apiKeyHasher, IOptions<ApiKeyCreateOption> createOptions) : DomainService
{
    public ApiKey Create(
        string key,
        string prefix,
        Guid userId,
        string name,
        string? description = null,
        bool isActive = true,
        DateTimeOffset? expirationTime = null)
    {
        if (prefix.Length != createOptions.Value.PrefixLength)
        {
            throw new ArgumentException(
                $"The prefix length must be {createOptions.Value.PrefixLength} characters.",
                nameof(prefix));
        }
        
        var hashedKey = apiKeyHasher.Hash(key);
        
        return new ApiKey(
            GuidGenerator.Create(),
            userId,
            prefix,
            name,
            description,
            isActive,
            expirationTime,
            CurrentTenant.Id)
        {
            Hash = hashedKey
        };
    }
    
    public bool Verify(string key, string hashedKey)
    {
        return apiKeyHasher.Verify(key, hashedKey);
    }

    public string GetPrefix(string key)
    {
        if (key.Length <= createOptions.Value.PrefixLength)
        {
            throw new ArgumentException(
                $"The key must be longer than {createOptions.Value.PrefixLength} characters to extract the prefix.",
                nameof(key));
        }
        
        return key[..createOptions.Value.PrefixLength];
    }

    public string GetRawKey(string key)
    {
        if (key.Length <= createOptions.Value.PrefixLength)
        {
            throw new ArgumentException(
                $"The key must be longer than {createOptions.Value.PrefixLength} characters to extract the key.",
                nameof(key));
        }
        
        return key[createOptions.Value.PrefixLength..];
    }
}
