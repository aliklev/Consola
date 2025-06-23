using ConSola.Infrastructure.DependencyInjection;
using ConSola.Core.Ports.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Terminal.Gui;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSerilog((serviceProvider, loggerConfig) => loggerConfig
            .ReadFrom.Configuration(context.Configuration));
        
        services.AddConSolaServices(context.Configuration);
        
        // Hexagonal Architecture - Core
        services.AddSingleton<ConSola.Core.UseCases.FileManagerService>();
        services.AddSingleton<ConSola.Core.Ports.Input.IFileManagerUseCase>(p => p.GetRequiredService<ConSola.Core.UseCases.FileManagerService>());
        
        // Output Adapters
        services.AddSingleton<ConSola.Core.Ports.Output.IFileSystemPort, ConSola.Adapters.Output.FileSystemAdapter>();
        

    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting ConSola application");
    
    Application.Init();
    
    var uiAdapter = host.Services.GetRequiredService<ConSola.Core.Ports.Output.IUserInterfacePort>();
    var inputHandler = host.Services.GetRequiredService<ConSola.Adapters.Input.ConsolaInput>();
    
    uiAdapter.Render();
    inputHandler.SetupKeyBindings();
    Application.Run();
}
catch (Exception ex)
{
    logger.LogError(ex, "Application failed");
}
finally
{
    Application.Shutdown();
    logger.LogInformation("Application shutdown complete");
}