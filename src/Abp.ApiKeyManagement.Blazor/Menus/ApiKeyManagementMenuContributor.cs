using System.Threading.Tasks;
using Abp.ApiKeyManagement.Localization;
using Abp.ApiKeyManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace Abp.ApiKeyManagement.Blazor.Menus;

public class ApiKeyManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ApiKeyManagementResource>();

        // Add API Keys menu item with permission check and localized display name
        context.Menu.AddItem(
            new ApplicationMenuItem(
                ApiKeyManagementMenus.ApiKeys,
                l["Menu:ApiKeyManagement"],
                url: "/api-key-management",
                icon: "fa fa-key",
                requiredPermissionName: ApiKeyManagementPermissions.ApiKeys.Default
            )
        );

        return Task.CompletedTask;
    }
}
