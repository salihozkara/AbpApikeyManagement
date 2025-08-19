using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
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
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(ApiKeyManagementMenus.ApiKeyManagement, displayName: "ApiKeyManagement", "~/ApiKeyManagement", icon: "fa fa-globe").RequireAuthenticated());

        return Task.CompletedTask;
    }
}
