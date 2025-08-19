using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyPermissionManagementProvider : PermissionManagementProvider
{
    public override string Name => ApiKeyPermissionValueProvider.ProviderName;
    
    public ApiKeyPermissionManagementProvider(IPermissionGrantRepository permissionGrantRepository, IGuidGenerator guidGenerator, ICurrentTenant currentTenant) : base(permissionGrantRepository, guidGenerator, currentTenant)
    {
    }
}