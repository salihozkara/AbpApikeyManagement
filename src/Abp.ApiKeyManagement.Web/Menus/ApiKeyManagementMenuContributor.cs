using System.Threading.Tasks;
using Abp.ApiKeyManagement.Localization;
using Abp.ApiKeyManagement.Permissions;
using Volo.Abp.UI.Navigation;

namespace Abp.ApiKeyManagement.Web.Menus;

public class ApiKeyManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ApiKeyManagementResource>();

        context.Menu.AddItem(
            new ApplicationMenuItem(
                ApiKeyManagementMenus.ApiKeys,
                l["Menu:ApiKeyManagement"],
                url: "~/ApiKeyManagement",
                icon: "fa fa-key",
                requiredPermissionName: ApiKeyManagementPermissions.ApiKeys.Default
            )
        );

        return Task.CompletedTask;
    }
}
