using System;
using System.ComponentModel.DataAnnotations;

namespace Abp.ApiKeyManagement.ApiKeys;

public class CreateApiKeyDto
{
    [Required]
    [StringLength(256, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? ExpirationTime { get; set; }
}