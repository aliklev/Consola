using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConSola.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConSolaServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core Use Cases
        services.AddSingleton<ConSola.Core.UseCases.FileManagerService>();
        services.AddSingleton<ConSola.Core.Ports.Input.IFileManagerUseCase>(p => 
            p.GetRequiredService<ConSola.Core.UseCases.FileManagerService>());
        
        // Output Adapters
        services.AddSingleton<ConSola.Core.Ports.Output.IFileSystemPort, 
            ConSola.Adapters.Output.FileSystemAdapter>();
        
        // Configuration
        services.Configure<ConSola.Core.Domain.UISettings>(configuration.GetSection("UI"));
        services.Configure<ConSola.Core.Domain.ApplicationSettings>(configuration.GetSection("Application"));
        
        // ConSola UI & Input
        services.AddSingleton<ConSola.Adapters.Output.ConsolaUI>();
        services.AddSingleton<ConSola.Core.Ports.Output.IUserInterfacePort>(p => 
            p.GetRequiredService<ConSola.Adapters.Output.ConsolaUI>());
        services.AddSingleton<ConSola.Adapters.Input.ConsolaInput>();
        
        return services;
    }
}