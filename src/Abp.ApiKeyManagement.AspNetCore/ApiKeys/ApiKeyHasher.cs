using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Abp.ApiKeyManagement.AspNetCore.ApiKeys;

/// <summary>
/// Infrastructure implementation of <see cref="IApiKeyHasher"/> using ASP.NET Core Identity's password hasher.
/// This implementation provides secure, one-way hashing for API keys using industry-standard algorithms.
/// </summary>
/// <remarks>
/// This implementation uses ASP.NET Core Identity's <see cref="IPasswordHasher{TUser}"/> which employs
/// PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, and 10,000 iterations by default.
/// The hashing algorithm is designed to be computationally expensive to resist brute-force attacks.
/// </remarks>
public class ApiKeyHasher(IPasswordHasher<object> passwordHasher) : IApiKeyHasher, ITransientDependency
{
    private readonly IPasswordHasher<object> _passwordHasher = Check.NotNull(passwordHasher, nameof(passwordHasher));

    /// <summary>
    /// Hashes an API key using a secure, one-way hashing algorithm.
    /// </summary>
    /// <param name="key">The plain-text API key to hash. Must not be null or empty.</param>
    /// <returns>A hashed representation of the API key that can be safely stored.</returns>
    /// <exception cref="System.ArgumentException">Thrown when <paramref name="key"/> is null or empty.</exception>
    /// <remarks>
    /// The resulting hash is non-reversible and suitable for secure storage.
    /// The same input will produce different hashes due to salt randomization.
    /// Use <see cref="Verify"/> to validate a plain-text key against a stored hash.
    /// </remarks>
    public string Hash(string key)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));

        return _passwordHasher.HashPassword(null!, key);
    }

    /// <summary>
    /// Verifies that a plain-text API key matches a previously hashed key.
    /// </summary>
    /// <param name="key">The plain-text API key to verify. Must not be null or empty.</param>
    /// <param name="hashedKey">The previously hashed API key to compare against. Must not be null or empty.</param>
    /// <returns>
    /// <c>true</c> if the plain-text key matches the hashed key; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="key"/> or <paramref name="hashedKey"/> is null or empty.
    /// </exception>
    /// <remarks>
    /// This method performs constant-time comparison to prevent timing attacks.
    /// It safely handles hash format upgrades and rehashing if needed.
    /// </remarks>
    public bool Verify(string key, string hashedKey)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNullOrWhiteSpace(hashedKey, nameof(hashedKey));

        var result = _passwordHasher.VerifyHashedPassword(null!, hashedKey, key);

        return result != PasswordVerificationResult.Failed;
    }
}
