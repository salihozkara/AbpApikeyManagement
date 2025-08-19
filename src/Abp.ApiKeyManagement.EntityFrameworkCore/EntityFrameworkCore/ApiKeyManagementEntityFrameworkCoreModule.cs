using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Abp.ApiKeyManagement.EntityFrameworkCore;

[DependsOn(
    typeof(ApiKeyManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ApiKeyManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ApiKeyManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<IApiKeyManagementDbContext>(includeAllEntities: true);
            
            options.AddRepository<ApiKey, EfCoreApiKeyRepository>();
        });
    }
}
