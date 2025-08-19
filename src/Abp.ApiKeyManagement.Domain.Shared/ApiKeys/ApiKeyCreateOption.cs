using System;
using System.Threading.Tasks;

namespace Abp.ApiKeyManagement.ApiKeys;

public class ApiKeyCreateOption
{
    public Func<ApiKeyCreateGenerationContext, Task<string>> PrefixGenerator { get; set; }
    
    public Func<ApiKeyCreateGenerationContext, Task<string>> KeyGenerator { get; set; }
    
    public int PrefixLength { get; set; }

    public ApiKeyCreateOption()
    {
        PrefixLength = 16;
        PrefixGenerator = _ => Task.FromResult(Guid.CreateVersion7().ToString("N")[..16]);
        KeyGenerator = _ => Task.FromResult(Guid.CreateVersion7().ToString("N"));
    }
}