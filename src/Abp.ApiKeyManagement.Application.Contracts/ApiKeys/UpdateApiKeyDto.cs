using System;

namespace Abp.ApiKeyManagement.ApiKeys;

public class UpdateApiKeyDto
{
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTimeOffset? ExpirationTime { get; set; }
}