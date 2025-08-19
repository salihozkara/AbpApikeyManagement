namespace Abp.ApiKeyManagement.ApiKeys;

public static class ApiKeyConsts
{
    public static int MaxNameLength { get; set; } = 64;
    public static int MaxDescriptionLength { get; set; } = 512;
    public static int MaxPrefixLength { get; set; } = 64;
    public static int MaxHashLength { get; set; } = 128;
}