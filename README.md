# ABP API Key Management Module

This module provides user-based API Key management for ABP Framework applications. Users can create API Keys that represent themselves by delegating permissions from their own authorization scope.

![API Keys Management](docs/images/api-keys.png)

## 🚀 Features

- **User-Based API Key Management**: Each user can create and manage their own API Keys
- **Permission-Based Access**: API Keys work only within the user's granted permissions
- **Secure Hash Storage**: API Keys are securely hashed and stored
- **Flexible Authentication**: API Key support via Headers and Query parameters
- **Expiration Support**: Optional expiration dates for API Keys
- **Multi-Tenant Support**: Full support for multi-tenant applications
- **Web UI**: User-friendly interface for managing API Keys

## 📋 Requirements

- ABP Framework 9.0+
- Entity Framework Core or MongoDB
- Distributed Cache (for performance)

## 🔧 Installation

### 1. Project References

Add the following references to the relevant layers of your project:

```xml
<!-- To Domain.Shared project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.Domain.Shared.csproj" />

<!-- To Domain project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.Domain.csproj" />

<!-- To Application.Contracts project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.Application.Contracts.csproj" />

<!-- To Application project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.Application.csproj" />

<!-- To EntityFrameworkCore project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.EntityFrameworkCore.csproj" />

<!-- To HttpApi project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.HttpApi.csproj" />

<!-- To Web project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.Web.csproj" />

<!-- To AspNetCore Host project -->
<ProjectReference Include="path\to\Abp.ApiKeyManagement.AspNetCore.csproj" />
```

### 2. Module Dependencies

Add the `DependsOn` attribute to your related module classes:

**Domain.Shared Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementDomainSharedModule)
)]
public class YourDomainSharedModule : AbpModule
{
}
```

**Domain Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementDomainModule)
)]
public class YourDomainModule : AbpModule
{
}
```

**Application.Contracts Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementApplicationContractsModule)
)]
public class YourApplicationContractsModule : AbpModule
{
}
```

**Application Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementApplicationModule)
)]
public class YourApplicationModule : AbpModule
{
}
```

**EntityFrameworkCore Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementEntityFrameworkCoreModule)
)]
public class YourEntityFrameworkCoreModule : AbpModule
{
}
```

**HttpApi Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementHttpApiModule)
)]
public class YourHttpApiModule : AbpModule
{
}
```

**Web Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(ApiKeyManagementWebModule)
)]
public class YourWebModule : AbpModule
{
}
```

**AspNetCore Host Module:**
```csharp
[DependsOn(
    // ... other dependencies
    typeof(AbpApiKeyManagementAspNetCoreModule)
)]
public class YourHostModule : AbpModule
{
}
```

### 3. Authentication Configuration

The API Key authentication is automatically configured when you add the `AbpApiKeyManagementAspNetCoreModule` dependency. No additional middleware configuration is required.

The module automatically:
- Registers the API Key authentication scheme
- Configures API Key resolvers for headers and query parameters
- Sets up authorization policies

Make sure your application has the standard ABP authentication/authorization middleware pipeline:

```csharp
public async override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
{
    var app = context.GetApplicationBuilder();
    var env = context.GetEnvironment();
    
    // ... other middlewares
    
    app.UseAuthentication(); // Standard ABP authentication
    app.UseAuthorization();  // Standard ABP authorization
    
    // ... other middlewares
}
```

### 4. DbContext Configuration

Add the following configuration to the `OnModelCreating` method of your `DbContext` class:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // ... other configurations
    
    modelBuilder.ConfigureApiKeyManagement();
}
```

### 5. Migration and Database Update

```bash
dotnet ef migrations add AddApiKeyManagement
dotnet ef database update
```

### 6. Optional Configurations

#### API Key Generation Options

Configure how API Keys are generated:

```csharp
public override void ConfigureServices(ServiceConfigurationContext context)
{
    Configure<ApiKeyCreateOption>(options =>
    {
        options.PrefixLength = 16; // Default: 16 characters
        
        // Custom prefix generator (optional)
        options.PrefixGenerator = context => Task.FromResult("myapp_" + Guid.NewGuid().ToString("N")[..10]);
        
        // Custom key generator (optional)
        options.KeyGenerator = context => Task.FromResult(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
    });
}
```

#### API Key Resolution Options

Configure how API Keys are resolved from HTTP requests:

```csharp
public override void ConfigureServices(ServiceConfigurationContext context)
{
    Configure<ApiKeyResolveOptions>(options =>
    {
        // Clear default resolvers if needed
        options.ApiKeyResolvers.Clear();
        
        // Add custom header resolvers
        options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("Authorization"));
        options.ApiKeyResolvers.Add(new HeaderApiKeyResolveContributor("X-Custom-Key"));
        
        // Add custom query parameter resolvers
        options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("token"));
        options.ApiKeyResolvers.Add(new QueryApiKeyResolveContributor("key"));
    });
}
```

**Default API Key Resolvers:**
- Headers: `X-Api-Key`, `Api-Key`
- Query Parameters: `apiKey`, `api_key`, `X-Api-Key`, `Api-Key`

## 📖 Usage

### Creating API Keys

![New API Key](docs/images/new-api-key.png)

Users can create new API Keys through the web interface:

1. Navigate to **API Key Management** page
2. Click **New API Key** button
3. Fill in the required information:
   - **Name**: Descriptive name for the API Key
   - **Description**: Optional description
   - **Expiration Date**: Optional expiration date
   - **Active**: Whether the API Key is active

![Created API Key](docs/images/created.png)
![Copy Message](docs/images/copy-message.png)

### Permission Management

![Permissions](docs/images/permissions.png)

You can manage permissions for API Keys:

1. Click **Permissions** button in the API Key list
2. Grant desired permissions from the user's available permissions to the API Key
3. The API Key will only work within the granted permissions scope

### Using API Keys

You can use the created API Keys in the following ways:

**Via Header:**
```http
GET /api/your-endpoint
X-Api-Key: your-api-key-here
```

**Via Query Parameter:**
```http
GET /api/your-endpoint?apiKey=your-api-key-here
```

## 🔒 Security

- API Keys are stored using secure hash algorithms
- Only permissions that the user has can be granted to API Keys
- API Keys can be limited with expiration dates
- Instant control with active/inactive status

## 🏗️ Architecture

This module follows ABP Framework's layered architecture:

- **Domain.Shared**: Constants, enums and shared types
- **Domain**: Business logic and domain services
- **Application.Contracts**: Application service interfaces and DTOs
- **Application**: Application services and business logic
- **EntityFrameworkCore**: Entity Framework Core integration
- **MongoDB**: MongoDB integration
- **HttpApi**: REST API controllers
- **HttpApi.Client**: HTTP client proxies
- **Web**: MVC web interface
- **AspNetCore**: Authentication and authorization integration

## 🛠️ Development

### Running Tests

```bash
dotnet test
```

### Building Project

```bash
dotnet build
```

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

You can open issues or start discussions for your questions.
