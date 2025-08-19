using System;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Users;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class ApiKeyPolicyHandler(ICurrentUser currentUser, IApiKeyStore apiKeyStore) : AuthorizationHandler<ApiKeyPolicyRequirement, string>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyPolicyRequirement requirement, string resource)
    {
        if (!currentUser.IsAuthenticated || !Guid.TryParse(resource, out var apiKeyId))
        {
            context.Fail();
            return;
        }

        var apiKey = await apiKeyStore.FindByIdAsync(apiKeyId);
        if (apiKey == null || apiKey.UserId != currentUser.Id)
        {
            context.Fail();
            return;
        }
        
        context.Succeed(requirement);
    }
    
    public class DefaultApiKeyPolicyHandler(ICurrentUser currentUser) : AuthorizationHandler<ApiKeyPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyPolicyRequirement requirement)
        {
            if (!currentUser.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }
        
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}