using System;
using System.ComponentModel.DataAnnotations;

namespace Abp.ApiKeyManagement.ApiKeys;

public class UpdateApiKeyDto
{
    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? ExpirationTime { get; set; }
}