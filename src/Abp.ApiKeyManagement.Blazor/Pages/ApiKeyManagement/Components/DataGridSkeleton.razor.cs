using Microsoft.AspNetCore.Components;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;

public partial class DataGridSkeleton : ApiKeyManagementComponentBase
{
    /// <summary>
    /// Number of skeleton rows to display
    /// </summary>
    [Parameter]
    public int Rows { get; set; } = 8;
}
