using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;

namespace Abp.ApiKeyManagement;

[DependsOn(
    typeof(ApiKeyManagementDomainModule),
    typeof(ApiKeyManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class ApiKeyManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<ApiKeyManagementApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ApiKeyManagementApplicationModule>(validate: true);
        });
        
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
