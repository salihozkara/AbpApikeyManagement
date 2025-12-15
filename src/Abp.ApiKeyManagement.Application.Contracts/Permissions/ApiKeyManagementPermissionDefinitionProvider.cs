using Abp.ApiKeyManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Abp.ApiKeyManagement.Permissions;

/// <summary>
/// Defines permissions for the API Key Management module.
/// Uses localized display names from ApiKeyManagementResource.
/// </summary>
public class ApiKeyManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var apiKeyManagementGroup = context.AddGroup(
            ApiKeyManagementPermissions.GroupName,
            L("Permission:ApiKeyManagement"));

        // API Keys permissions
        var apiKeysPermission = apiKeyManagementGroup.AddPermission(
            ApiKeyManagementPermissions.ApiKeys.Default,
            L("Permission:ApiKeys"));

        apiKeysPermission.AddChild(
            ApiKeyManagementPermissions.ApiKeys.Create,
            L("Permission:ApiKeys.Create"));

        apiKeysPermission.AddChild(
            ApiKeyManagementPermissions.ApiKeys.Edit,
            L("Permission:ApiKeys.Edit"));

        apiKeysPermission.AddChild(
            ApiKeyManagementPermissions.ApiKeys.Delete,
            L("Permission:ApiKeys.Delete"));

        apiKeysPermission.AddChild(
            ApiKeyManagementPermissions.ApiKeys.ManagePermissions,
            L("Permission:ApiKeys.ManagePermissions"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ApiKeyManagementResource>(name);
    }
}
