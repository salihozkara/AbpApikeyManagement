using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.AspNetCore.ApiKeys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Abp.ApiKeyManagement.AspNetCore;

[DependsOn(typeof(AbpAspNetCoreMvcModule),
    typeof(ApiKeyManagementDomainSharedModule))]
public class AbpApiKeyManagementAspNetCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthorizationConsts.AuthenticationType, null);

        context.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
        
        Configure<ApiKeyResolveOptions>(options =>
        {
            options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("X-Api-Key"));
            options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("Api-Key"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("apiKey"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("api_key"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("X-Api-Key"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("Api-Key"));
        });

        context.Services.AddAuthorizationCore(o => o.AddPolicy("apiKeyManagement", p =>
        {
            p.Requirements.Add(new ApiKeyPolicyRequirement());
        }));
        context.Services.AddScoped<IAuthorizationHandler, ApiKeyPolicyHandler>();
        context.Services.AddScoped<IAuthorizationHandler, ApiKeyPolicyHandler.DefaultApiKeyPolicyHandler>();
    }
}