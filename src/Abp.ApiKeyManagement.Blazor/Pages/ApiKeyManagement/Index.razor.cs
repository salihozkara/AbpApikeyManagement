using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;
using Abp.ApiKeyManagement.Permissions;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Volo.Abp.PermissionManagement.Blazor.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement;

public partial class Index : ApiKeyManagementComponentBase
{
    #region Injected Services

    [Inject]
    protected IApiKeyAppService ApiKeyAppService { get; set; } = default!;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    // Note: The following are inherited from AbpComponentBase:
    // - IStringLocalizer L (configured with ApiKeyManagementResource in base class)
    // - IAuthorizationService AuthorizationService
    // - IUiMessageService Message
    // - IObjectMapper ObjectMapper

    #endregion

    #region Component References

    protected TextEdit SearchInputRef { get; set; } = default!;

    #endregion

    #region Modal References

    protected CreateApiKeyModal CreateModalRef { get; set; } = default!;
    protected EditApiKeyModal EditModalRef { get; set; } = default!;
    protected ApiKeyRevealModal RevealModalRef { get; set; } = default!;
    protected Volo.Abp.PermissionManagement.Blazor.Components.PermissionManagementModal PermissionModalRef { get; set; } = default!;

    #endregion

    #region Breadcrumbs

    protected List<BreadcrumbItem> BreadcrumbItems { get; } = new();

    #endregion

    #region Permissions

    protected bool HasCreatePermission { get; set; }
    protected bool HasEditPermission { get; set; }
    protected bool HasDeletePermission { get; set; }
    protected bool HasManagePermissionsPermission { get; set; }

    #endregion

    #region Data Grid State

    protected List<ApiKeyDto> ApiKeysList { get; set; } = new();
    protected int TotalCount { get; set; }
    protected int CurrentPage { get; set; } = 1;
    protected int PageSize { get; set; } = 10;
    protected string CurrentSorting { get; set; } = $"{nameof(ApiKeyDto.CreationTime)} desc";
    protected string FilterText { get; set; } = string.Empty;

    #endregion

    #region Advanced Filters

    protected bool? FilterIsActive { get; set; }
    protected DateTimeOffset? FilterExpirationFrom { get; set; }
    protected DateTimeOffset? FilterExpirationTo { get; set; }
    protected bool ShowFilterPanel { get; set; }

    /// <summary>
    /// Gets the count of active filters (excluding text search)
    /// </summary>
    protected int ActiveFilterCount =>
        (FilterIsActive.HasValue ? 1 : 0) +
        (FilterExpirationFrom.HasValue || FilterExpirationTo.HasValue ? 1 : 0);

    /// <summary>
    /// Checks if any filters are active
    /// </summary>
    protected bool HasActiveFilters => ActiveFilterCount > 0 || !string.IsNullOrWhiteSpace(FilterText);

    #endregion

    #region Search Debounce

    private Timer? _searchDebounceTimer;
    private const int SearchDebounceMs = 300;

    #endregion

    #region Lifecycle Methods

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:ApiKeyManagement"]));

        await SetPermissionsAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _searchDebounceTimer?.Dispose();
        }
        base.Dispose(disposing);
    }

    #endregion

    #region Permission Setup

    protected virtual async Task SetPermissionsAsync()
    {
        HasCreatePermission = await AuthorizationService.IsGrantedAsync(
            ApiKeyManagementPermissions.ApiKeys.Create);

        HasEditPermission = await AuthorizationService.IsGrantedAsync(
            ApiKeyManagementPermissions.ApiKeys.Edit);

        HasDeletePermission = await AuthorizationService.IsGrantedAsync(
            ApiKeyManagementPermissions.ApiKeys.Delete);

        HasManagePermissionsPermission = await AuthorizationService.IsGrantedAsync(
            ApiKeyManagementPermissions.ApiKeys.ManagePermissions);
    }

    #endregion

    #region Data Loading

    protected virtual async Task LoadApiKeysAsync()
    {
        try
        {
            var result = await ApiKeyAppService.GetListAsync(new GetApiKeyListDto
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting,
                Name = string.IsNullOrWhiteSpace(FilterText) ? null : FilterText, // Pass null for empty filter
                IsActive = FilterIsActive, // Filter by status
                ExpirationTime = FilterExpirationTo // Filter by expiration date
            });

            ApiKeysList = result.Items.ToList();
            TotalCount = (int)result.TotalCount;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ApiKeyDto> e)
    {
        CurrentPage = e.Page;
        PageSize = e.PageSize;

        // Handle sorting
        if (e.Columns != null)
        {
            var sortByColumns = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .OrderBy(c => c.SortIndex)
                .Select(c => $"{c.Field} {(c.SortDirection == SortDirection.Descending ? "desc" : "asc")}")
                .ToList();

            CurrentSorting = sortByColumns.Count != 0
                ? string.Join(",", sortByColumns)
                : $"{nameof(ApiKeyDto.CreationTime)} desc";
        }

        await LoadApiKeysAsync();
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Search

    protected virtual void OnSearchTextChanged(ChangeEventArgs e)
    {
        FilterText = e.Value?.ToString() ?? string.Empty;

        // Debounce search - wait 300ms after last keystroke
        _searchDebounceTimer?.Dispose();
        _searchDebounceTimer = new Timer(async _ =>
        {
            await InvokeAsync(async () =>
            {
                CurrentPage = 1; // Reset to first page on search
                await LoadApiKeysAsync();
                StateHasChanged();
            });
        }, null, SearchDebounceMs, Timeout.Infinite);
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    #endregion

    #region Status Badge Helper

    /// <summary>
    /// Determines the badge color, icon, and text based on the API key status
    /// </summary>
    protected virtual (Color badgeColor, IconName icon, string text) GetStatusBadge(ApiKeyDto apiKey)
    {
        // Check if inactive
        if (!apiKey.IsActive)
        {
            return (Color.Secondary, IconName.Ban, "Inactive");
        }

        // Check if expired
        if (apiKey.ExpirationTime.HasValue && apiKey.ExpirationTime.Value < DateTimeOffset.Now)
        {
            return (Color.Danger, IconName.Clock, "Expired");
        }

        // Check if expiring soon (within 7 days)
        if (apiKey.ExpirationTime.HasValue &&
            apiKey.ExpirationTime.Value < DateTimeOffset.Now.AddDays(7))
        {
            return (Color.Warning, IconName.ExclamationTriangle, "ExpiresSoon");
        }

        // Active
        return (Color.Success, IconName.Check, "Active");
    }

    #endregion

    #region CRUD Operations

    protected virtual async Task OpenCreateModalAsync()
    {
        await CreateModalRef.ShowAsync();
    }

    protected virtual async Task OpenEditModalAsync(ApiKeyDto apiKey)
    {
        await EditModalRef.ShowAsync(apiKey.Id);
    }

    protected virtual async Task DeleteApiKeyAsync(ApiKeyDto apiKey)
    {
        try
        {
            await ApiKeyAppService.DeleteAsync(apiKey.Id);
            await Message.Success(L["ApiKeyDeleted"]);
            await LoadApiKeysAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    #endregion

    #region Modal Callbacks

    /// <summary>
    /// Handles the successful creation of an API key
    /// Shows the RevealModal with the new key
    /// </summary>
    protected virtual async Task OnApiKeyCreatedAsync(ApiKeyCreateResultDto result)
    {
        try
        {
            // Show the API key reveal modal (one-time display)
            await RevealModalRef.ShowAsync(result.Key);

            // Refresh the grid to show the new key
            await LoadApiKeysAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    /// <summary>
    /// Handles the successful update of an API key
    /// </summary>
    protected virtual async Task OnApiKeyUpdatedAsync()
    {
        try
        {
            // Refresh the grid to show updated data
            await LoadApiKeysAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    #endregion

    #region Permission Management

    /// <summary>
    /// Opens the permission management modal for an API key using ABP's built-in modal
    /// </summary>
    protected virtual async Task OpenPermissionModalAsync(ApiKeyDto apiKey)
    {
        try
        {
            // Use ABP's built-in PermissionManagementModal
            await PermissionModalRef.OpenAsync(
                providerName: ApiKeyAuthorizationConsts.PermissionValueProviderName,
                providerKey: apiKey.Id.ToString(),
                entityDisplayName: apiKey.Name
            );
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    #endregion

    #region Keyboard Navigation & Accessibility

    /// <summary>
    /// Handles keyboard shortcuts for search input
    /// </summary>
    protected virtual async Task HandleSearchKeyDown(KeyboardEventArgs e)
    {
        // Enter key triggers search
        if (e.Key == "Enter")
        {
            await SearchAsync();
        }
    }

    /// <summary>
    /// Registers global keyboard shortcuts
    /// NOTE: This would be called from OnAfterRenderAsync if implementing global shortcuts
    /// For now, documenting the pattern for future enhancement
    /// </summary>
    /*
    private async Task RegisterKeyboardShortcuts()
    {
        // Ctrl+K / Cmd+K: Focus search
        // Alt+N: Open new API key modal
        // Escape: Close modals (handled by modal components)

        await JSRuntime.InvokeVoidAsync("registerKeyboardShortcuts", DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task FocusSearchInput()
    {
        if (SearchInputRef != null)
        {
            await SearchInputRef.Focus();
        }
    }

    [JSInvokable]
    public async Task OpenNewApiKeyModal()
    {
        if (HasCreatePermission)
        {
            await OpenCreateModalAsync();
        }
    }
    */

    #endregion

    #region Filter Management

    /// <summary>
    /// Toggles the filter panel visibility
    /// </summary>
    protected virtual void ToggleFilterPanel()
    {
        ShowFilterPanel = !ShowFilterPanel;
    }

    /// <summary>
    /// Applies status filter
    /// </summary>
    protected virtual async Task ApplyStatusFilterAsync(bool? isActive)
    {
        FilterIsActive = isActive;
        CurrentPage = 1; // Reset to first page
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Applies date range filter (quick preset)
    /// </summary>
    protected virtual async Task ApplyDateFilterAsync(int? daysFromNow)
    {
        if (daysFromNow.HasValue)
        {
            FilterExpirationFrom = DateTimeOffset.Now;
            FilterExpirationTo = DateTimeOffset.Now.AddDays(daysFromNow.Value);
        }
        else
        {
            FilterExpirationFrom = null;
            FilterExpirationTo = null;
        }

        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Handles changes to the "From" date picker
    /// </summary>
    protected virtual async Task OnExpirationFromChangedAsync(DateTimeOffset? date)
    {
        FilterExpirationFrom = date;
        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Handles changes to the "To" date picker
    /// </summary>
    protected virtual async Task OnExpirationToChangedAsync(DateTimeOffset? date)
    {
        FilterExpirationTo = date;
        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Removes a specific filter
    /// </summary>
    protected virtual async Task RemoveFilterAsync(string filterName)
    {
        switch (filterName.ToLower())
        {
            case "status":
                FilterIsActive = null;
                break;
            case "date":
            case "expiration":
                FilterExpirationFrom = null;
                FilterExpirationTo = null;
                break;
            case "text":
            case "search":
                FilterText = string.Empty;
                break;
        }

        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Clears all active filters
    /// </summary>
    protected virtual async Task ClearAllFiltersAsync()
    {
        FilterText = string.Empty;
        FilterIsActive = null;
        FilterExpirationFrom = null;
        FilterExpirationTo = null;
        CurrentPage = 1;
        await LoadApiKeysAsync();
    }

    /// <summary>
    /// Gets the display text for status filter
    /// </summary>
    protected virtual string GetStatusFilterText()
    {
        if (!FilterIsActive.HasValue) return string.Empty;
        return FilterIsActive.Value ? L["Active"] : L["Inactive"];
    }

    /// <summary>
    /// Gets the display text for date filter
    /// </summary>
    protected virtual string GetDateFilterText()
    {
        if (!FilterExpirationFrom.HasValue && !FilterExpirationTo.HasValue)
            return string.Empty;

        if (FilterExpirationFrom.HasValue && FilterExpirationTo.HasValue)
        {
            var days = (FilterExpirationTo.Value - FilterExpirationFrom.Value).Days;
            if (days == 7) return L["Last7Days"];
            if (days == 30) return L["Last30Days"];
            return $"{FilterExpirationFrom.Value:d} - {FilterExpirationTo.Value:d}";
        }

        if (FilterExpirationTo.HasValue)
            return $"{L["ExpiringBefore"]} {FilterExpirationTo.Value:d}";

        return $"{L["ExpiringAfter"]} {FilterExpirationFrom.Value:d}";
    }

    #endregion

    #region Inline Status Toggle

    /// <summary>
    /// Toggles the IsActive status of an API key inline (optimistic update)
    /// </summary>
    protected virtual async Task UpdateIsActiveAsync(ApiKeyDto apiKey, bool newValue)
    {
        // Store original value for rollback
        var originalValue = apiKey.IsActive;

        try
        {
            // Optimistic update: immediately update local state
            apiKey.IsActive = newValue;
            StateHasChanged();

            // Call backend to persist the change
            await ApiKeyAppService.UpdateAsync(apiKey.Id, new UpdateApiKeyDto
            {
                Description = apiKey.Description,
                IsActive = newValue,
                ExpirationTime = apiKey.ExpirationTime
            });

            // Success notification
            var statusText = newValue ? L["Active"] : L["Inactive"];
            await Message.Success(string.Format(L["ApiKeyStatusUpdated"], apiKey.Name, statusText));
        }
        catch (Exception ex)
        {
            // Rollback on error
            apiKey.IsActive = originalValue;
            StateHasChanged();

            await HandleErrorAsync(ex);
        }
    }

    #endregion
}
