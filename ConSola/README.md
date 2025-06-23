# ConSola - Norton Commander Clone

A modern C# console file manager built with .NET 9 using **Hexagonal Architecture**.

## Architecture

### Hexagonal (Ports & Adapters) Pattern

```
┌─────────────────────────────────────────────────────────────┐
│                    INPUT ADAPTERS                           │
│  ┌─────────────────┐    ┌─────────────────────────────────┐ │
│  │ ConsoleInput    │    │ NavigationAdapter               │ │
│  │ Adapter         │    │                                 │ │
│  └─────────────────┘    └─────────────────────────────────┘ │
└─────────────────┬───────────────────────────────────┬───────┘
                  │                                   │
┌─────────────────▼───────────────────────────────────▼───────┐
│                        CORE                                 │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                   DOMAIN                                │ │
│  │  ┌─────────────┐                                        │ │
│  │  │  FileItem   │                                        │ │
│  │  └─────────────┘                                        │ │
│  └─────────────────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                 USE CASES                               │ │
│  │  ┌─────────────────────────────────────────────────────┐ │ │
│  │  │           FileManagerService                        │ │ │
│  │  └─────────────────────────────────────────────────────┘ │ │
│  └─────────────────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                    PORTS                                │ │
│  │  Input: IFileManagerUseCase, INavigationUseCase        │ │
│  │  Output: IFileSystemPort, IUserInterfacePort           │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────┬───────────────────────────────────┬───────┘
                  │                                   │
┌─────────────────▼───────────────────────────────────▼───────┐
│                   OUTPUT ADAPTERS                           │
│  ┌─────────────────┐    ┌─────────────────────────────────┐ │
│  │ FileSystem      │    │ ConsoleUI                       │ │
│  │ Adapter         │    │ Adapter                         │ │
│  └─────────────────┘    └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

```
ConSola/
├── Core/                           # Business Logic (Framework Independent)
│   ├── Domain/                     # Domain Entities
│   │   └── FileItem.cs
│   ├── UseCases/                   # Business Logic
│   │   └── FileManagerService.cs
│   └── Ports/                      # Interfaces
│       ├── Input/                  # What Core Accepts
│       │   ├── IFileManagerUseCase.cs
│       │   └── INavigationUseCase.cs
│       └── Output/                 # What Core Produces
│           ├── IFileSystemPort.cs
│           └── IUserInterfacePort.cs
├── Adapters/                       # External Concerns
│   ├── Input/                      # Primary Adapters (Drivers)
│   │   ├── ConsoleInputAdapter.cs
│   │   └── NavigationAdapter.cs
│   └── Output/                     # Secondary Adapters (Driven)
│       ├── FileSystemAdapter.cs
│       └── ConsoleUIAdapter.cs
└── Infrastructure/                 # Cross-cutting Concerns
    └── DependencyInjection/
        └── ServiceCollectionExtensions.cs
```

## Features

- **Two-panel file manager** (Norton Commander style)
- **Keyboard navigation** (Enter, Tab, F5, F6, F8, Ctrl+D)
- **File operations** (Copy, Move, Delete)
- **Drive switching** (Ctrl+D)
- **Async file operations**
- **Clean Architecture** (Hexagonal/Ports & Adapters)
- **Dependency Injection** (Microsoft.Extensions.DI)

## Key Bindings

| Key | Action |
|-----|--------|
| `Enter` | Navigate into directory |
| `Tab` | Switch between panels |
| `Backspace` | Navigate up one level |
| `F5` | Copy selected item |
| `F6` | Move selected item |
| `F8` | Delete selected item |
| `Ctrl+D` | Switch drive |
| `F1` | Show help |
| `F10/Esc` | Exit |

## Usage

```bash
# Build and run
dotnet build
dotnet run
```

## Architecture Benefits

1. **Testability** - Core logic isolated from external dependencies
2. **Flexibility** - Easy to swap UI frameworks or file systems
3. **Maintainability** - Clear separation of concerns
4. **SOLID Principles** - Each component has single responsibility
5. **Clean Dependencies** - Core has no external framework dependencies

## Dependencies

- .NET 9
- Terminal.Gui (Console UI)
- Microsoft.Extensions.Hosting (DI & Configuration)
- Serilog (Logging)

## License

MIT License