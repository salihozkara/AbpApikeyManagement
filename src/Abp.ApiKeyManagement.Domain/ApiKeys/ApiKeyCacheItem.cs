using System;

namespace Abp.ApiKeyManagement.ApiKeys;

[Serializable]
public class ApiKeyCacheItem
{
    public bool IsFound { get; set; }
    public ApiKeyInfo? ApiKey { get; set; }
    
    public static ApiKeyCacheItem NotFound => new() { IsFound = false };
}