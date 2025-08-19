using Microsoft.AspNetCore.Http;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class HeaderApiKeyResolveContributor(string headerName) : IApiKeyResolveContributor
{
    public bool Select(HttpContext httpContext)
    {
        return httpContext.Request.Headers.ContainsKey(headerName);
    }

    public string Resolve(HttpContext httpContext)
    {
        return httpContext.Request.Headers[headerName].ToString();
    }
}