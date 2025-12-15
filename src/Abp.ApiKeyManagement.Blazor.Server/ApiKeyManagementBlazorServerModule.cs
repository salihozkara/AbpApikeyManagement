using Abp.ApiKeyManagement.AspNetCore;
using Abp.ApiKeyManagement.Blazor;
using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Blazor.Server;

namespace Abp.ApiKeyManagement;

/// <summary>
/// Blazor Server module for API Key Management.
/// Wraps the base Blazor module with Server-specific configuration and authentication.
/// </summary>
/// <remarks>
/// This module follows ABP's modular architecture pattern:
/// <list type="bullet">
/// <item><description>Depends on base Blazor module (shared UI components)</description></item>
/// <item><description>Adds AspNetCore module for API key authentication handler</description></item>
/// <item><description>Adds Server-specific theming and permission management</description></item>
/// </list>
/// Use this module for Blazor Server render mode applications.
/// </remarks>
[DependsOn(
    typeof(ApiKeyManagementBlazorModule),
    typeof(AbpApiKeyManagementAspNetCoreModule),
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(AbpPermissionManagementBlazorServerModule)
)]
public class ApiKeyManagementBlazorServerModule : AbpModule
{
    // Configuration is inherited from base ApiKeyManagementBlazorModule
    // AspNetCore module provides API key authentication handler
    // Server theming and permission management modules provide Server-specific UI components
}
