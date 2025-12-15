using Abp.ApiKeyManagement.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Abp.ApiKeyManagement.Blazor;

public abstract class ApiKeyManagementComponentBase : AbpComponentBase
{
    protected ApiKeyManagementComponentBase()
    {
        LocalizationResource = typeof(ApiKeyManagementResource);
    }
}