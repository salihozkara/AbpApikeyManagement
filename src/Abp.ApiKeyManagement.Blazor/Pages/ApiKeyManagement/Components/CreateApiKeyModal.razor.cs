using System;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.Localization;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;

public partial class CreateApiKeyModal : ApiKeyManagementComponentBase
{
    #region Injected Services

    [Inject]
    protected IApiKeyAppService ApiKeyAppService { get; set; } = default!;

    [Inject]
    protected AbpBlazorMessageLocalizerHelper<ApiKeyManagementResource> LH { get; set; } = default!;

    #endregion

    #region Parameters

    [Parameter]
    public EventCallback<ApiKeyCreateResultDto> OnSaved { get; set; }

    #endregion

    #region Component References

    protected Modal ModalRef { get; set; } = default!;
    protected Validations ValidationsRef { get; set; } = default!;
    protected TextEdit FirstInputRef { get; set; } = default!;

    #endregion

    #region State

    protected CreateApiKeyViewModel ViewModel { get; set; } = new();
    protected bool IsSaving { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Opens the modal for creating a new API key
    /// </summary>
    public virtual async Task ShowAsync()
    {
        try
        {
            // Reset form
            ViewModel = new CreateApiKeyViewModel
            {
                IsActive = true // Default to active
            };

            // Clear validation
            if (ValidationsRef != null)
            {
                await ValidationsRef.ClearAll();
            }

            // Show modal
            await ModalRef.Show();

            // Focus first input after modal animation completes
            await Task.Delay(300);
            if (FirstInputRef != null)
            {
                await FirstInputRef.Focus();
            }
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
    /// Saves the new API key
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
            var dto = mapper.MapToCreateDto(ViewModel);

            // Call API
            var result = await ApiKeyAppService.CreateAsync(dto);

            // Success notification
            await Message.Success(L["ApiKeyCreated"]);

            // Close modal
            await ModalRef.Hide();

            // Notify parent component
            if (OnSaved.HasDelegate)
            {
                await OnSaved.InvokeAsync(result);
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
