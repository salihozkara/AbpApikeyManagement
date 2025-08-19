using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Abp.ApiKeyManagement.ApiKeys;

[Serializable]
public class ApiKey : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid UserId { get; protected set; }

    public string Name
    {
        get;
        protected set => field = Check.NotNullOrWhiteSpace(value, nameof(value), ApiKeyConsts.MaxNameLength);
    } = string.Empty;

    public string? Description
    {
        get;
        set => field = Check.Length(value, nameof(value), ApiKeyConsts.MaxDescriptionLength);
    }

    public bool IsActive { get; set; }
    public Guid? TenantId { get; protected set; }
    
    public string Prefix { get; protected internal set => field = Check.NotNullOrWhiteSpace(value, nameof(value), ApiKeyConsts.MaxPrefixLength); } = string.Empty;
    
    public string Hash { get; protected internal set; } = string.Empty;

    public DateTimeOffset? ExpirationTime { get; set; }

    protected ApiKey()
    {
    }

    public ApiKey
    (
        Guid id,
        Guid userId,
        string prefix,
        string name,
        string? description = null,
        bool isActive = true,
        DateTimeOffset? expirationTime = null,
        Guid? tenantId = null
    ) : base(id)
    {
        UserId = userId;
        Prefix = prefix;
        Name = name;
        Description = description;
        IsActive = isActive;
        ExpirationTime = expirationTime;
        TenantId = tenantId;
    }
}