using System;
using Volo.Abp.Application.Dtos;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    
    public string Prefix { get; set; } = string.Empty;
    
    public DateTimeOffset? ExpirationTime { get; set; }
    
    public DateTime CreationTime { get; set; }
}