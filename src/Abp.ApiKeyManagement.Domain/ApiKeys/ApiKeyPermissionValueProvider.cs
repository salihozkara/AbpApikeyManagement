using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyPermissionValueProvider(IPermissionStore permissionStore, IPermissionValueProviderManager permissionValueProviderManager) : PermissionValueProvider(permissionStore)
{
    public const string ProviderName = ApiKeyAuthorizationConsts.PermissionValueProviderName;
    public override string Name => ProviderName;

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var authenticationType = context.Principal?.Identity?.AuthenticationType;

        if (authenticationType != ApiKeyAuthorizationConsts.AuthenticationType)
        {
            return PermissionGrantResult.Undefined;
        }
        
        var apiKeyId = context.Principal?.FindFirst(ApiKeyClaimTypes.ApiKeyId)?.Value;
        
        if (apiKeyId == null)
        {
            return PermissionGrantResult.Undefined;
        }

        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        
        if (userId == null)
        {
            return PermissionGrantResult.Undefined;
        }

        var otherProviders = permissionValueProviderManager.ValueProviders.Where(x => x.Name != Name);
        var anyGranted = false;
        foreach (var permissionValueProvider in otherProviders)
        {
            var result = await permissionValueProvider.CheckAsync(context);
            if (result != PermissionGrantResult.Granted)
            {
                continue;
            }
            
            anyGranted = true;
            break;
        }

        if (!anyGranted)
        {
            return PermissionGrantResult.Prohibited;
        }

        return await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, apiKeyId)
            ? PermissionGrantResult.Granted
            : PermissionGrantResult.Prohibited;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToArray();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));
        
        
        var authenticationType = context.Principal?.Identity?.AuthenticationType;
        
        if (authenticationType != ApiKeyAuthorizationConsts.AuthenticationType)
        {
            return new MultiplePermissionGrantResult(permissionNames);
        }
        
        var apiKeyPrefix = context.Principal?.FindFirst(ApiKeyClaimTypes.ApiKeyId)?.Value;
        if (apiKeyPrefix == null)
        {
            return new MultiplePermissionGrantResult(permissionNames);
        }
        
        var userId = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        if (userId == null)
        {
            return new MultiplePermissionGrantResult(permissionNames);
        }

        var result = new MultiplePermissionGrantResult();
        
        var otherProviders = permissionValueProviderManager.ValueProviders.Where(x => x.Name != Name);
        var otherPermissionGrantResults = new List<MultiplePermissionGrantResult>();
        foreach (var permissionValueProvider in otherProviders)
        {
            var otherResult = await permissionValueProvider.CheckAsync(context);
            if (otherResult.Result.Count == 0)
            {
                continue;
            }
            otherPermissionGrantResults.Add(otherResult);
        }
        
        foreach (var permissionName in permissionNames)
        {
            var anyGranted = false;
            foreach (var otherResult in otherPermissionGrantResults)
            {
                if (!otherResult.Result.TryGetValue(permissionName, out var permissionGrantResult) ||
                    permissionGrantResult != PermissionGrantResult.Granted)
                {
                    continue;
                }
                
                anyGranted = true;
                break;
            }
            
            if (!anyGranted)
            {
                result.Result.Add(permissionName, PermissionGrantResult.Prohibited);
                continue;
            }
            
            var isGranted = await PermissionStore.IsGrantedAsync(permissionName, Name, apiKeyPrefix);
            result.Result.Add(permissionName, isGranted ? PermissionGrantResult.Granted : PermissionGrantResult.Prohibited);
        }
        
        return result;
    }
}