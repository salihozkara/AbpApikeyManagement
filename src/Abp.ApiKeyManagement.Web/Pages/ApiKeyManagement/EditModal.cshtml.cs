using System;
using System.Threading.Tasks;
using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abp.ApiKeyManagement.Web.Pages.ApiKeyManagement;

[Authorize]
public class EditModal : ApiKeyManagementPageModel
{
    [BindProperty(SupportsGet = true)] 
    public Guid Id { get; set; }
    
    [BindProperty]
    public EditApiKeyViewModel ApiKey { get; set; } = new();

    private readonly IApiKeyAppService _apiKeyAppService;

    public EditModal(IApiKeyAppService apiKeyAppService)
    {
        _apiKeyAppService = apiKeyAppService;
    }

    public async Task OnGetAsync()
    {
        ApiKey = ObjectMapper.Map<ApiKeyDto, EditApiKeyViewModel>(await _apiKeyAppService.GetAsync(Id));
    }
    
    public Task OnPostAsync()
    {
        var dto = ObjectMapper.Map<EditApiKeyViewModel, UpdateApiKeyDto>(ApiKey);
        return _apiKeyAppService.UpdateAsync(Id, dto);
    }

    public class EditApiKeyViewModel
    {
        public string? Description { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}