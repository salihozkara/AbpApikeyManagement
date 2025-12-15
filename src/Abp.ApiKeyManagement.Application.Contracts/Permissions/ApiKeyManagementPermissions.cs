using Volo.Abp.Reflection;

namespace Abp.ApiKeyManagement.Permissions;

/// <summary>
/// Permission constants for the API Key Management module.
/// Follows ABP's hierarchical permission pattern.
/// </summary>
public class ApiKeyManagementPermissions
{
    public const string GroupName = "ApiKeyManagement";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ApiKeyManagementPermissions));
    }

    /// <summary>
    /// Permissions for API Keys management
    /// </summary>
    public static class ApiKeys
    {
        public const string Default = GroupName + ".ApiKeys";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }
}
