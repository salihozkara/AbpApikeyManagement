using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;

public partial class FilterChip : ApiKeyManagementComponentBase
{
    #region Parameters

    /// <summary>
    /// The display label for the filter
    /// </summary>
    [Parameter]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The filter value to display
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// The type of filter for color coding and icons
    /// </summary>
    [Parameter]
    public FilterChipType Type { get; set; } = FilterChipType.General;

    /// <summary>
    /// Callback when the chip is removed
    /// </summary>
    [Parameter]
    public EventCallback OnRemove { get; set; }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the appropriate color based on filter type
    /// </summary>
    private Color GetChipColor()
    {
        return Type switch
        {
            FilterChipType.Status => Color.Primary,
            FilterChipType.Date => Color.Warning,
            FilterChipType.Text => Color.Info,
            _ => Color.Secondary
        };
    }

    /// <summary>
    /// Gets the appropriate icon based on filter type
    /// </summary>
    private IconName GetChipIcon()
    {
        return Type switch
        {
            FilterChipType.Status => IconName.Tag,
            FilterChipType.Date => IconName.Calendar,
            FilterChipType.Text => IconName.Search,
            _ => IconName.Filter
        };
    }

    /// <summary>
    /// Handles the remove button click
    /// </summary>
    private async Task OnRemoveClicked()
    {
        if (OnRemove.HasDelegate)
        {
            await OnRemove.InvokeAsync();
        }
    }

    #endregion
}

/// <summary>
/// Enum defining filter chip types for visual styling
/// </summary>
public enum FilterChipType
{
    General,
    Status,
    Date,
    Text
}
