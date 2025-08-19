using Microsoft.AspNetCore.Http;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public interface IApiKeyResolveContributor
{
    bool Select(HttpContext httpContext);
    
    string Resolve(HttpContext httpContext);
}