using Abp.ApiKeyManagement.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Abp.ApiKeyManagement.EntityFrameworkCore;

public static class ApiKeyManagementDbContextModelCreatingExtensions
{
    public static void ConfigureApiKeyManagement(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<ApiKey>(b =>
        {
            // Configure table and schema name
            b.ToTable(ApiKeyManagementDbProperties.DbTablePrefix + "ApiKeys", ApiKeyManagementDbProperties.DbSchema);
            
            b.ConfigureByConvention(); //autoconfigure for the base class props
            
            // Properties
            b.Property(a => a.Name).IsRequired().HasMaxLength(ApiKeyConsts.MaxNameLength);
            b.Property(a => a.IsActive).IsRequired();
            b.Property(a => a.ExpirationTime).IsRequired(false);
            b.Property(a => a.Prefix).IsRequired().HasMaxLength(ApiKeyConsts.MaxPrefixLength);
            b.Property(a => a.Hash).IsRequired().HasMaxLength(ApiKeyConsts.MaxHashLength);
            b.Property(a => a.UserId).IsRequired();
            b.Property(a => a.Description).IsRequired(false).HasMaxLength(ApiKeyConsts.MaxDescriptionLength);
            
            // Indexes
            b.HasIndex(a => a.Prefix).IsUnique();
            b.HasIndex(a => a.UserId);
            b.HasIndex(a => new { a.UserId, a.Name }).IsUnique();
            b.HasIndex(a => new { a.UserId, a.Prefix });
            b.HasIndex(a => a.CreationTime);
        });
    }
}
