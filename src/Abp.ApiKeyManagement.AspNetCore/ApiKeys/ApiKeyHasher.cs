using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.DependencyInjection;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

public class ApiKeyHasher(IPasswordHasher<object> passwordHasher) : IApiKeyHasher, ITransientDependency
{
    public string Hash(string key)
    {
        return passwordHasher.HashPassword(null!, key);
    }

    public bool Verify(string key, string hashedKey)
    {
        return passwordHasher.VerifyHashedPassword(null!, hashedKey, key) != PasswordVerificationResult.Failed;
    }
}