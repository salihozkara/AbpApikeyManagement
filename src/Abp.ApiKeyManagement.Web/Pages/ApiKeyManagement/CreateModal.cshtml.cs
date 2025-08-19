using System;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Abp.ApiKeyManagement.Web.Pages.ApiKeyManagement;

[Authorize]
public class CreateModal : ApiKeyManagementPageModel
{
    [BindProperty]
    public CreateApiKeyViewModel ApiKey { get; set; } = new();

    private readonly IApiKeyAppService _apiKeyAppService;

    public CreateModal(IApiKeyAppService apiKeyAppService)
    {
        _apiKeyAppService = apiKeyAppService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<CreateApiKeyViewModel, CreateApiKeyDto>(ApiKey);
        return new OkObjectResult(await _apiKeyAppService.CreateAsync(dto));
    }
    
    public class CreateApiKeyViewModel
    {
        public string Name { get; set; } = string.Empty;
        
        [TextArea(Rows = 3)]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
    }
}