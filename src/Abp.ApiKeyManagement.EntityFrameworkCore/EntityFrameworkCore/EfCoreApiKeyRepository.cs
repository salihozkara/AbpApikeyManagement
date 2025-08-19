using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Abp.ApiKeyManagement.EntityFrameworkCore;

public class EfCoreApiKeyRepository : EfCoreRepository<ApiKeyManagementDbContext, ApiKey, Guid>, IApiKeyRepository
{
    public EfCoreApiKeyRepository(IDbContextProvider<ApiKeyManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(string? filter = null, Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(filter, userId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<ApiKey>> GetListAsync(string? name = null, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(name, userId);
        return await query
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<ApiKey>> GetPagedListAsync(int skipCount = 0, int maxResultCount = int.MaxValue, string? sorting = null, string? name = null, Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(name, userId);
        return await query
            .OrderBy(sorting ?? nameof(ApiKey.CreationTime) + " desc")
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<ApiKey?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.FirstOrDefaultAsync(x => x.Prefix == prefix, GetCancellationToken(cancellationToken));
    }

    public async Task<ApiKey?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.FirstOrDefaultAsync(x => x.Name == name && x.UserId == userId, GetCancellationToken(cancellationToken));
    }

    protected virtual async Task<IQueryable<ApiKey>> GetQueryableAsync(
        string? name,
        Guid? userId)
    {
        var query = (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(name), x => x.Name.Contains(name!))
            .WhereIf(userId.HasValue, x => x.UserId == userId);

        return query;
    }
}