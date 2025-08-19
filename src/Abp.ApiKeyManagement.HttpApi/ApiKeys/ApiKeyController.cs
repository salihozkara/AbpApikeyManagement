using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Abp.ApiKeyManagement.ApiKeys;

[Area(ApiKeyManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ApiKeyManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/api-key-management/api-keys")]
[Authorize]
public class ApiKeyController(IApiKeyAppService apiKeyAppService) :  ApiKeyManagementController, IApiKeyAppService
{
    [HttpGet]
    [Route("{id:guid}")]
    public Task<ApiKeyDto> GetAsync(Guid id)
    {
        return apiKeyAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<ApiKeyCreateResultDto> CreateAsync(CreateApiKeyDto input)
    {
        return apiKeyAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public Task<ApiKeyDto> UpdateAsync(Guid id, UpdateApiKeyDto input)
    {
        return apiKeyAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public Task DeleteAsync(Guid id)
    {
        return apiKeyAppService.DeleteAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ApiKeyDto>> GetListAsync(GetApiKeyListDto input)
    {
        return apiKeyAppService.GetListAsync(input);
    }
}