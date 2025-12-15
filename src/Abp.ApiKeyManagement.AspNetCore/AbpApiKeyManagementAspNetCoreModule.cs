using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.AspNetCore.ApiKeys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Abp.ApiKeyManagement.AspNetCore;

/// <summary>
/// ASP.NET Core module for API Key Management.
/// Provides HTTP authentication infrastructure including request handlers, API key resolvers, and hashing services.
/// </summary>
/// <remarks>
/// This module handles all ASP.NET Core specific concerns:
/// <list type="bullet">
/// <item><description>API key authentication handler for HTTP requests</description></item>
/// <item><description>API key resolution from headers and query strings</description></item>
/// <item><description>API key hashing using ASP.NET Core Identity's password hasher</description></item>
/// </list>
/// Authorization is handled by ABP's permission system in the Application layer.
/// </remarks>
[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(ApiKeyManagementDomainSharedModule))]
public class AbpApiKeyManagementAspNetCoreModule : AbpModule
{
    /// <summary>
    /// Configures ASP.NET Core specific services for API key management.
    /// Sets up authentication schemes and API key resolution from HTTP requests.
    /// </summary>
    /// <param name="context">Service configuration context provided by ABP framework.</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Register password hasher for API key hashing (moved from Infrastructure module)
        context.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();

        // Register API key authentication scheme for HTTP requests
        context.Services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthorizationConsts.AuthenticationType,
                options => { });

        // Configure API key resolution from various HTTP sources (headers and query parameters)
        Configure<ApiKeyResolveOptions>(options =>
        {
            // Standard HTTP header resolvers
            options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("X-Api-Key"));
            options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("Api-Key"));

            // Query string parameter resolvers (useful for scenarios where headers cannot be set)
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("apiKey"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("api_key"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("X-Api-Key"));
            options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("Api-Key"));
        });
    }
}