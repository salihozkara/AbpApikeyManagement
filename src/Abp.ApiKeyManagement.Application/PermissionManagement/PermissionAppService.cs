using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace Abp.ApiKeyManagement.PermissionManagement;

[Authorize]
[Dependency(ReplaceServices = false)]
public class PermissionAppService : ApplicationService, IPermissionAppService
{
    protected virtual IPermissionAppService InnerService { get; }
    protected virtual IApiKeyStore ApiKeyStore { get; }
    protected virtual PermissionManagementOptions Options { get; }
    public PermissionAppService(IPermissionAppService innerService, IOptions<PermissionManagementOptions> options, IApiKeyStore apiKeyStore)
    {
        ApiKeyStore = apiKeyStore ?? throw new ArgumentNullException(nameof(apiKeyStore));
        InnerService = innerService;
        Options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
    {
        await CheckProviderPolicy(providerName, providerKey);
        
        var result = await InnerService.GetAsync(providerName, providerKey);
        if(providerName != ApiKeyPermissionValueProvider.ProviderName || !Guid.TryParse(providerKey, out var id))
        {
            return result;
        }

        var apiKey = await ApiKeyStore.FindByIdAsync(id);
        if (apiKey == null)
        {
            return result;
        }
        
        var userPermissions = await InnerService.GetAsync(UserPermissionValueProvider.ProviderName, apiKey.UserId.ToString());
        
        var deletedGroups = new List<PermissionGroupDto>();
        foreach (var permission in result.Groups)
        {
            var userGroup = userPermissions.Groups.Find(g => g.Name == permission.Name);
            if (userGroup == null)
            {
                deletedGroups.Add(permission);
                continue;
            }
            
            var deletedPermissions = new List<PermissionGrantInfoDto>();
            foreach (var p in permission.Permissions)
            {
                var userPermission = userGroup.Permissions.Find(up => up.Name == p.Name);
                if (userPermission is not { IsGranted: true })
                {
                    deletedPermissions.Add(p);
                }
            }
            
            permission.Permissions.RemoveAll(p => deletedPermissions.Contains(p));
            if (permission.Permissions.Count == 0)
            {
                deletedGroups.Add(permission);
            }
        }
        
        result.Groups.RemoveAll(g => deletedGroups.Contains(g));
        
        return result;
    }

    public async Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {
        await CheckProviderPolicy(providerName, providerKey);
        await InnerService.UpdateAsync(providerName, providerKey, input);
    }

    protected virtual async Task CheckProviderPolicy(string providerName, string providerKey)
    {
        var policyName = Options.ProviderPolicies.GetOrDefault(providerName);
        if (policyName.IsNullOrEmpty())
        {
            throw new AbpException($"No policy defined to get/set permissions for the provider '{providerName}'. Use {nameof(PermissionManagementOptions)} to map the policy.");
        }

        await AuthorizationService.CheckAsync(providerKey, policyName);
    }
}