using System;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyInfo
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? TenantId { get; set; }
    public DateTimeOffset? ExpirationTime { get; set; }
    
    public string Prefix { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}