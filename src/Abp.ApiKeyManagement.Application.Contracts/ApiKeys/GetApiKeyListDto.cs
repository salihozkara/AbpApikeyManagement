using System;
using Volo.Abp.Application.Dtos;

namespace Abp.ApiKeyManagement.ApiKeys;

public class GetApiKeyListDto : PagedAndSortedResultRequestDto
{
    public string Name { get; set; } = string.Empty;
    
    public bool? IsActive { get; set; }
    
    public DateTimeOffset? ExpirationTime { get; set; }
}