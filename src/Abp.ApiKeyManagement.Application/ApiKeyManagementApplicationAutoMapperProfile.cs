using System.Linq;
using Abp.ApiKeyManagement.ApiKeys;
using AutoMapper;
using Volo.Abp.AutoMapper;

namespace Abp.ApiKeyManagement;

public class ApiKeyManagementApplicationAutoMapperProfile : Profile
{
    public ApiKeyManagementApplicationAutoMapperProfile()
    {
        CreateMap<ApiKey, ApiKeyDto>()
            .ForMember(x => x.Prefix, opt => opt.MapFrom(src => new string(src.Prefix.Take(5).ToArray()) + "..."));
        CreateMap<ApiKey, ApiKeyInfo>();
        CreateMap<ApiKey, ApiKeyCreateResultDto>()
            .IncludeBase<ApiKey, ApiKeyDto>()
            .Ignore(x => x.Key);
    }
}
