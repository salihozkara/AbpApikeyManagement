using System;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.Localization;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;

public partial class EditApiKeyModal : ApiKeyManagementComponentBase
{
    #region Injected Services

    [Inject]
    protected IApiKeyAppService ApiKeyAppService { get; set; } = default!;

    [Inject]
    protected AbpBlazorMessageLocalizerHelper<ApiKeyManagementResource> LH { get; set; } = default!;

    #endregion

    #region Parameters

    [Parameter]
    public EventCallback OnSaved { get; set; }

    #endregion

    #region Component References

    protected Modal ModalRef { get; set; } = default!;
    protected Validations ValidationsRef { get; set; } = default!;

    #endregion

    #region State

    protected EditApiKeyViewModel ViewModel { get; set; } = new();
    protected Guid EditingApiKeyId { get; set; }
    protected string ApiKeyName { get; set; } = string.Empty;
    protected bool IsSaving { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Opens the modal for editing an existing API key
    /// </summary>
    public virtual async Task ShowAsync(Guid apiKeyId)
    {
        try
        {
            EditingApiKeyId = apiKeyId;

            // Load existing data
            var apiKey = await ApiKeyAppService.GetAsync(apiKeyId);

            // Store name for display (readonly field)
            ApiKeyName = apiKey.Name;

            // Map to ViewModel using Mapperly
            var mapper = new ApiKeyManagementBlazorMapper();
            ViewModel = mapper.MapToEditViewModel(apiKey);

            // Clear validation
            if (ValidationsRef != null)
            {
                await ValidationsRef.ClearAll();
            }

            // Show modal
            await ModalRef.Show();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    /// <summary>
    /// Closes the modal
    /// </summary>
    public virtual async Task CloseAsync()
    {
        try
        {
            await ModalRef.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Saves the changes to the API key
    /// </summary>
    protected virtual async Task SaveAsync()
    {
        try
        {
            // Validate form
            if (!await ValidationsRef.ValidateAll())
            {
                return;
            }

            IsSaving = true;
            await InvokeAsync(StateHasChanged);

            // Map ViewModel to DTO using Mapperly
            var mapper = new ApiKeyManagementBlazorMapper();
            var dto = mapper.MapToUpdateDto(ViewModel);

            // Call API
            await ApiKeyAppService.UpdateAsync(EditingApiKeyId, dto);

            // Success notification
            await Message.Success(L["ApiKeyUpdated"]);

            // Close modal
            await ModalRef.Hide();

            // Notify parent component
            if (OnSaved.HasDelegate)
            {
                await OnSaved.InvokeAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsSaving = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    #endregion
}
