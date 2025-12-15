using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace Abp.ApiKeyManagement.Blazor.WebAssembly;

[DependsOn(
    typeof(ApiKeyManagementBlazorModule),
    typeof(ApiKeyManagementHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class ApiKeyManagementBlazorWebAssemblyModule : AbpModule
{

}
