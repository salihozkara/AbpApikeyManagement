using Abp.ApiKeyManagement.ApiKeys;
using Abp.ApiKeyManagement.Web.Pages.ApiKeyManagement;
using AutoMapper;

namespace Abp.ApiKeyManagement.Web;

public class ApiKeyManagementWebAutoMapperProfile : Profile
{
    public ApiKeyManagementWebAutoMapperProfile()
    {
        CreateMap<CreateModal.CreateApiKeyViewModel, CreateApiKeyDto>();
        CreateMap<EditModal.EditApiKeyViewModel, UpdateApiKeyDto>();
        CreateMap<ApiKeyDto, EditModal.EditApiKeyViewModel>();
    }
}
