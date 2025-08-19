using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyAppService
{
    Task<ApiKeyDto> GetAsync(Guid id);
    Task<ApiKeyCreateResultDto> CreateAsync(CreateApiKeyDto input);
    Task<ApiKeyDto> UpdateAsync(Guid id, UpdateApiKeyDto input);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<ApiKeyDto>> GetListAsync(GetApiKeyListDto input);
}