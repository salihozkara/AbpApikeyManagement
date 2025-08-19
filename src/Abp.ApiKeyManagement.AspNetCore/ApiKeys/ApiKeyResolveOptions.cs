using System.Collections.Generic;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class ApiKeyResolveOptions
{
    public List<IApiKeyResolveContributor> ApiKeyResolvers { get; } = [];
}