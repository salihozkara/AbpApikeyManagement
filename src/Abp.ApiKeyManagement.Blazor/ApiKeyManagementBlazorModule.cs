using Abp.ApiKeyManagement.Blazor.Menus;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Blazor;
using Volo.Abp.UI.Navigation;

namespace Abp.ApiKeyManagement.Blazor;

/// <summary>
/// Blazor UI module for API Key Management.
/// Provides reusable Blazor components, pages, and navigation for managing API keys.
/// </summary>
/// <remarks>
/// This module contains only UI components and depends on Application.Contracts for service interfaces and DTOs.
/// It works seamlessly in both Blazor Server and Blazor WebAssembly modes through ABP's HTTP client proxy system.
/// </remarks>
[DependsOn(
    typeof(ApiKeyManagementApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpPermissionManagementBlazorModule),
    typeof(AbpMapperlyModule)
    )]
public class ApiKeyManagementBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<ApiKeyManagementBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ApiKeyManagementMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(ApiKeyManagementBlazorModule).Assembly);
        });
    }
}
