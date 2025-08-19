using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.ApiKeyManagement.EntityFrameworkCore;

[ConnectionStringName(ApiKeyManagementDbProperties.ConnectionStringName)]
public class ApiKeyManagementDbContext : AbpDbContext<ApiKeyManagementDbContext>, IApiKeyManagementDbContext
{
    public DbSet<ApiKey> ApiKeys { get; set; }

    public ApiKeyManagementDbContext(DbContextOptions<ApiKeyManagementDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureApiKeyManagement();
    }
}
