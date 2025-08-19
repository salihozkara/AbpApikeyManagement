using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyStore
{
    Task<ApiKeyInfo?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task<ApiKeyInfo?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}