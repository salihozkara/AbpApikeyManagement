using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Abp.ApiKeyManagement.EntityFrameworkCore;

[ConnectionStringName(ApiKeyManagementDbProperties.ConnectionStringName)]
public interface IApiKeyManagementDbContext : IEfCoreDbContext
{
    DbSet<ApiKey> ApiKeys { get; set; }
}
