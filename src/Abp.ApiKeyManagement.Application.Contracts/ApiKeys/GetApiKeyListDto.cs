using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Abp.ApiKeyManagement.ApiKeys;

public class GetApiKeyListDto : PagedAndSortedResultRequestDto
{
    [StringLength(256)]
    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public DateTimeOffset? ExpirationTime { get; set; }
}