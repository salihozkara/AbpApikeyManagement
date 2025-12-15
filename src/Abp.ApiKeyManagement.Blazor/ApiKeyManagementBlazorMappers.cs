using System;
using System.ComponentModel.DataAnnotations;
using Abp.ApiKeyManagement.ApiKeys;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Abp.ApiKeyManagement.Blazor;

/// <summary>
/// View model for creating a new API key
/// </summary>
public class CreateApiKeyViewModel
{
    [Required]
    [StringLength(256, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset? ExpirationTime { get; set; }
}

/// <summary>
/// View model for editing an existing API key
/// </summary>
public class EditApiKeyViewModel
{
    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? ExpirationTime { get; set; }
}

/// <summary>
/// Mapperly mapper for API Key Management Blazor view models
/// </summary>
[Mapper]
public partial class ApiKeyManagementBlazorMapper
{
    // Map from CreateApiKeyViewModel to CreateApiKeyDto
    public partial CreateApiKeyDto MapToCreateDto(CreateApiKeyViewModel viewModel);

    // Map from EditApiKeyViewModel to UpdateApiKeyDto
    public partial UpdateApiKeyDto MapToUpdateDto(EditApiKeyViewModel viewModel);

    // Map from ApiKeyDto to EditApiKeyViewModel (only editable fields - warnings are expected for readonly fields)
    public partial EditApiKeyViewModel MapToEditViewModel(ApiKeyDto dto);
}