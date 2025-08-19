namespace Abp.ApiKeyManagement.ApiKeys;

public interface IApiKeyHasher
{
    string Hash(string key);
    bool Verify(string key, string hashedKey);
}