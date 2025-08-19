using System.Collections.Generic;
using Abp.ApiKeyManagement.ApiKeys;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Abp.ApiKeyManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ApiKeyManagementDomainSharedModule)
)]
public class ApiKeyManagementDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<PermissionManagementOptions>(options =>
        {
            options.ManagementProviders.Add<ApiKeyPermissionManagementProvider>();
            options.ProviderPolicies[ApiKeyPermissionValueProvider.ProviderName] = "apiKeyManagement";
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpPermissionOptions>(opt =>
        {
            opt.ValueProviders.AddFirst(typeof(ApiKeyPermissionValueProvider));
        });
    }
}
