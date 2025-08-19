using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyRepository : IBasicRepository<ApiKey, Guid>
{
    Task<long> GetCountAsync(string? name = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<List<ApiKey>> GetListAsync(string? name = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<List<ApiKey>> GetPagedListAsync(int skipCount = 0, int maxResultCount = int.MaxValue, string? sorting = null, string? name = null, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ApiKey?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task<ApiKey?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken = default);
}