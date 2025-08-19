using System.Security.Claims;
using System.Threading.Tasks;

namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyPrincipalProvider
{
    Task<ClaimsPrincipal?> GetApiKeyPrincipalOrNullAsync(string key);
}


