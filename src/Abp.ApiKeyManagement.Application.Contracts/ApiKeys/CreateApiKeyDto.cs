using System;

namespace Abp.ApiKeyManagement.ApiKeys;

public class CreateApiKeyDto
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTimeOffset? ExpirationTime { get; set; }
}