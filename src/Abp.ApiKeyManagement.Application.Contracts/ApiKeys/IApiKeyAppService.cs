using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyAppService : IApplicationService
{
    Task<ApiKeyDto> GetAsync(Guid id);
    Task<ApiKeyCreateResultDto> CreateAsync(CreateApiKeyDto input);
    Task<ApiKeyDto> UpdateAsync(Guid id, UpdateApiKeyDto input);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<ApiKeyDto>> GetListAsync(GetApiKeyListDto input);
}