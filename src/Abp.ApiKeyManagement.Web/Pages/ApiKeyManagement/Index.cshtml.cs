using Microsoft.AspNetCore.Authorization;

namespace Abp.ApiKeyManagement.Web.Pages.ApiKeyManagement;

[Authorize]
public class IndexModel : ApiKeyManagementPageModel
{
    public void OnGet()
    {
    }
}
