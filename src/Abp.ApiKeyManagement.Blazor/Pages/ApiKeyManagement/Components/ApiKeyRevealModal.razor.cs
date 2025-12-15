using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Abp.ApiKeyManagement.Blazor.Pages.ApiKeyManagement.Components;

public partial class ApiKeyRevealModal : ApiKeyManagementComponentBase
{
    #region Injected Services

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    #endregion

    #region Component References

    protected Modal ModalRef { get; set; } = default!;
    protected TextEdit KeyInputRef { get; set; } = default!;

    #endregion

    #region State

    protected string ApiKey { get; set; } = string.Empty;
    protected bool Copied { get; set; }
    protected bool UserAcknowledged { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Opens the modal and displays the API key (one-time only)
    /// </summary>
    public virtual async Task ShowAsync(string apiKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await Message.Error(L["InvalidApiKey"]);
                return;
            }

            // Set state
            ApiKey = apiKey;
            Copied = false;
            UserAcknowledged = false;

            // Show modal
            await ModalRef.Show();

            // Auto-select the key text after modal animation (slight delay)
            await Task.Delay(300);
            await SelectKeyTextAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    /// <summary>
    /// Closes the modal (only allowed after user acknowledgment)
    /// </summary>
    public virtual async Task CloseAsync()
    {
        try
        {
            if (!UserAcknowledged)
            {
                await Message.Warn(L["PleaseAcknowledgeCopy"]);
                return;
            }

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
    /// Copies the API key to clipboard using modern Clipboard API
    /// </summary>
    protected virtual async Task CopyToClipboardAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", ApiKey);

            Copied = true;
            StateHasChanged();

            await Message.Success(L["ApiKeyCopied"]);
        }
        catch (Exception)
        {
            // Fallback: try to select the text so user can copy manually
            await SelectKeyTextAsync();
            await Message.Warn(L["CopyToClipboardFailed"]);
        }
    }

    /// <summary>
    /// Auto-selects the API key text for easy copying
    /// </summary>
    protected virtual async Task SelectKeyTextAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("eval",
                @"
                const input = document.getElementById('apiKeyInput');
                if (input) {
                    input.select();
                    input.setSelectionRange(0, 99999); // For mobile devices
                }
                ");
        }
        catch (Exception)
        {
            // Silently fail - non-critical feature
        }
    }

    /// <summary>
    /// Prevents modal from closing without acknowledgment
    /// </summary>
    protected virtual Task OnModalClosing(ModalClosingEventArgs e)
    {
        if (!UserAcknowledged)
        {
            e.Cancel = true;
            return InvokeAsync(async () =>
            {
                await Message.Warn(L["PleaseAcknowledgeCopy"]);
            });
        }

        return Task.CompletedTask;
    }

    #endregion
}
