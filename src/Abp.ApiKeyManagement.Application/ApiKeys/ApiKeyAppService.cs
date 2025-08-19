using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Users;

namespace Abp.ApiKeyManagement.ApiKeys;

[Authorize]
public class ApiKeyAppService(IApiKeyRepository apiKeyRepository, ApiKeyManager apiKeyManager, IOptions<ApiKeyCreateOption> createOptions) : ApiKeyManagementAppService, IApiKeyAppService
{
    public async Task<ApiKeyDto> GetAsync(Guid id)
    {
        var apiKey = await apiKeyRepository.GetAsync(id);
        
        if(apiKey.UserId != CurrentUser.GetId())
        {
            throw new AbpAuthorizationException("You are not authorized to access this API key.");
        }
        
        return ObjectMapper.Map<ApiKey, ApiKeyDto>(apiKey);
    }

    public async Task<ApiKeyCreateResultDto> CreateAsync(CreateApiKeyDto input)
    {
        var createContext = new ApiKeyCreateGenerationContext
        {
            Name = input.Name,
            Description = input.Description,
            IsActive = input.IsActive,
            ExpirationTime = input.ExpirationTime,
            UserId = CurrentUser.GetId(),
            TenantId = CurrentTenant.Id,
            ServiceProvider = LazyServiceProvider
        };
        var key = await createOptions.Value.KeyGenerator(createContext);
        var prefix = await createOptions.Value.PrefixGenerator(createContext);
        
        await CheckNameAlreadyExistsAsync(input.Name);
        await CheckPrefixAlreadyExistsAsync(prefix);
        
        var apiKey = apiKeyManager.Create(key, prefix, CurrentUser.GetId(), input.Name, input.Description, input.IsActive, input.ExpirationTime);
        await apiKeyRepository.InsertAsync(apiKey, true);
        
        var apiKeyDto = ObjectMapper.Map<ApiKey, ApiKeyCreateResultDto>(apiKey);
        apiKeyDto.Key = prefix + key;
        
        return apiKeyDto;
    }

    public async Task<ApiKeyDto> UpdateAsync(Guid id, UpdateApiKeyDto input)
    {
        var apiKey = await apiKeyRepository.GetAsync(id);
        if (apiKey.UserId != CurrentUser.GetId())
        {
            throw new AbpAuthorizationException("You are not authorized to update this API key.");
        }
        
        apiKey.Description = input.Description;
        apiKey.IsActive = input.IsActive;
        apiKey.ExpirationTime = input.ExpirationTime;
        await apiKeyRepository.UpdateAsync(apiKey, true);
        return ObjectMapper.Map<ApiKey, ApiKeyDto>(apiKey);
    }

    public async Task DeleteAsync(Guid id)
    {
        var apiKey = await apiKeyRepository.GetAsync(id);
        if (apiKey.UserId != CurrentUser.GetId())
        {
            throw new AbpAuthorizationException("You are not authorized to delete this API key.");
        }
        
        await apiKeyRepository.DeleteAsync(apiKey, true);
    }

    public async Task<PagedResultDto<ApiKeyDto>> GetListAsync(GetApiKeyListDto input)
    {
        var count = await apiKeyRepository.GetCountAsync(input.Name, CurrentUser.GetId());
        
        if (count <= 0)
        {
            return new PagedResultDto<ApiKeyDto>(0, []);
        }
        
        var apiKeys = await apiKeyRepository.GetPagedListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Name,
            CurrentUser.Id
        );

        return new PagedResultDto<ApiKeyDto>(
            count,
            ObjectMapper.Map<List<ApiKey>, List<ApiKeyDto>>(apiKeys)
        );
    }
    
    protected virtual async Task CheckNameAlreadyExistsAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        var apiKey = await apiKeyRepository.FindByNameAsync(name, CurrentUser.GetId());

        if (apiKey != null)
        {
            throw new AbpException($"An API key with the name '{name}' already exists.");
        }
    }
    
    protected virtual async Task CheckPrefixAlreadyExistsAsync(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix, nameof(prefix));
        var apiKey = await apiKeyRepository.FindByPrefixAsync(prefix);

        if (apiKey != null)
        {
            throw new AbpException($"An API key with the prefix '{prefix}' already exists.");
        }
    }
}