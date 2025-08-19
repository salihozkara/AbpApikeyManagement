using System;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyCreateGenerationContext
{
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? ExpirationTime { get; set; }
    
    public required IServiceProvider ServiceProvider { get; set; }
}