using System.Linq;
using Abp.ApiKeyManagement.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Abp.ApiKeyManagement;

/// <summary>
/// Application module for API Key Management.
/// Contains application services that orchestrate domain logic and provide APIs for UI consumption.
/// </summary>
/// <remarks>
/// <para>
/// <strong>AspNetCore Module Dependency:</strong>
/// This module declares a dependency on <see cref="AbpApiKeyManagementAspNetCoreModule"/> for ABP module
/// initialization ordering. The AspNetCore module registers the <see cref="ApiKeys.IApiKeyHasher"/>
/// implementation and authentication handlers required by domain services.
/// </para>
/// <para>
/// <strong>Note on Clean Architecture:</strong>
/// This is a <em>module-level</em> dependency for DI container registration, not a code-level coupling.
/// Application layer code never imports or uses AspNetCore types directly. All interactions occur
/// through interfaces defined in Domain.Shared (<see cref="ApiKeys.IApiKeyHasher"/>,
/// <see cref="ApiKeys.IApiKeyStore"/>). The project reference exists solely because ABP's
/// <see cref="DependsOnAttribute"/> requires compile-time type access to the module class.
/// </para>
/// </remarks>
[DependsOn(
    typeof(ApiKeyManagementDomainModule),
    typeof(ApiKeyManagementApplicationContractsModule),
    typeof(AbpApiKeyManagementAspNetCoreModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpPermissionManagementApplicationModule)
)]
public class ApiKeyManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<ApiKeyManagementApplicationModule>();
        
        var descriptor = context.Services.First(sd => sd.ServiceType == typeof(IPermissionAppService) && sd.ServiceKey == null);
        
        var originalFactory = descriptor.ImplementationFactory;
        context.Services.Remove(descriptor);
        
        context.Services.Add(new ServiceDescriptor(typeof(IPermissionAppService), sp =>
        {
            var original = originalFactory?.Invoke(sp) 
                           ?? ActivatorUtilities.CreateInstance(sp, descriptor.ImplementationType!);
            var injector = sp.GetRequiredService<IInjectPropertiesService>();

            injector.InjectUnsetProperties(original);

            var decorator = ActivatorUtilities.CreateInstance<Abp.ApiKeyManagement.PermissionManagement.PermissionAppService>(sp, original);
            injector.InjectUnsetProperties(decorator);

            return decorator;
        }, descriptor.Lifetime));

    }
}
