using Microsoft.AspNetCore.Http;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class QueryApiKeyResolveContributor(string queryParamName) : IApiKeyResolveContributor
{
    public bool Select(HttpContext httpContext)
    {
        return httpContext.Request.Query.ContainsKey(queryParamName);
    }

    public string Resolve(HttpContext httpContext)
    {
        return httpContext.Request.Query[queryParamName].ToString();
    }
}