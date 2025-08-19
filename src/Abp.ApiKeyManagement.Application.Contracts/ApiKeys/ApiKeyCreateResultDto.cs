namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyCreateResultDto : ApiKeyDto
{
    public string Key { get; set; } = string.Empty;
}