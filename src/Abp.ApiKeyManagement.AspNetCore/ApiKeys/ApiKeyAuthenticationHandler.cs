using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<ApiKeyResolveOptions> apiKeyResolveOptions,
    IApiKeyPrincipalProvider principalProvider)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder), IAuthenticationRequestHandler
{
    private readonly ApiKeyResolveOptions _apiKeyResolveOptions = apiKeyResolveOptions.Value;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var resolver = _apiKeyResolveOptions.ApiKeyResolvers
            .FirstOrDefault(r => r.Select(Request.HttpContext));

        if (resolver == null)
        {
            return AuthenticateResult.NoResult();
        }
        
        var apiKey = resolver.Resolve(Request.HttpContext);
        
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var principal = await principalProvider.GetApiKeyPrincipalOrNullAsync(apiKey!);

        if (principal == null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    public async Task<bool> HandleRequestAsync()
    {
        var result = await HandleAuthenticateAsync();
        if (result?.Principal != null)
        {
            Context.User = result.Principal;
        }
        if (result?.Succeeded ?? false)
        {
            var authFeatures = new AuthenticationFeatures(result);
            Context.Features.Set<IHttpAuthenticationFeature>(authFeatures);
            Context.Features.Set<IAuthenticateResultFeature>(authFeatures);
        }

        return false;
    }

    private sealed class AuthenticationFeatures : IAuthenticateResultFeature, IHttpAuthenticationFeature
    {
        private ClaimsPrincipal? _user;
        private AuthenticateResult? _result;

        public AuthenticationFeatures(AuthenticateResult result)
        {
            AuthenticateResult = result;
        }

        public AuthenticateResult? AuthenticateResult
        {
            get => _result;
            set
            {
                _result = value;
                _user = _result?.Principal;
            }
        }

        public ClaimsPrincipal? User
        {
            get => _user;
            set
            {
                _user = value;
                _result = null;
            }
        }
    }
}