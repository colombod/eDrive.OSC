# eDrive.OSC - Open Sound Control Library for .NET

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Essential Setup
- Install .NET 9.0 SDK (REQUIRED - system likely has .NET 8.0 by default):
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0
  export PATH="$HOME/.dotnet:$PATH"
  export DOTNET_ROOT="$HOME/.dotnet"
  dotnet --version  # Should show 9.0.x
  ```
- Navigate to: `cd source/eDrive.OSC`

### Build and Test
- Build all projects: `dotnet build` -- takes 2 seconds. NEVER CANCEL. Set timeout to 10+ seconds.
- Build for release: `dotnet build --configuration Release` -- takes 2 seconds. NEVER CANCEL. Set timeout to 10+ seconds.
- Run all tests: `cd eDrive.OSC.Tests && dotnet test --logger console` -- takes 6 seconds, 193 tests. NEVER CANCEL. Set timeout to 30+ seconds.
- Individual project builds work from their directories: `cd eDrive.OSC && dotnet build`

### Validation
- ALWAYS run both build and test commands after making changes.
- All 193 tests should pass (100% test coverage claimed in documentation).
- Build warnings are acceptable (currently 11 warnings about unused variables and nullable references).
- The build system automatically handles project dependencies.

### Manual Functionality Testing
After changes, validate the core functionality works:
```csharp
// Quick smoke test - should compile and run without errors
var message = new OscMessage("/test");
message.Append(42f);
byte[] data = message.ToByteArray();
Console.WriteLine($"OSC message serialized to {data.Length} bytes");
```

## Architecture

### Project Structure
```
source/eDrive.OSC/
├── eDrive.OSC/                    # Core OSC library (messages, bundles, serialization)
├── eDrive.OSC.Interfaces/         # Core interfaces for transport abstraction  
├── eDrive.OSC.Network/            # UDP, TCP, HTTP transport implementations
├── eDrive.OSC.Network.NamedPipes/ # Windows Named Pipes transport
├── eDrive.OSC.Reactive/           # Reactive Extensions (Rx.NET) integration
├── eDrive.OSC.Tests/              # Comprehensive test suite (193 tests)
└── eDrive.OSC.slnx               # Visual Studio solution file
```

### Key Projects by Functionality
- **eDrive.OSC**: Core functionality - OSC messages, bundles, binary/JSON serialization
- **eDrive.OSC.Network**: Network transports for UDP, TCP, HTTP protocols  
- **eDrive.OSC.Network.NamedPipes**: Windows-specific Named Pipes transport
- **eDrive.OSC.Reactive**: Reactive Extensions integration for event-driven applications
- **eDrive.OSC.Interfaces**: Interfaces enabling transport abstraction and dependency injection
- **eDrive.OSC.Tests**: xUnit test suite with FluentAssertions

## Common Tasks

### Building Individual Projects
```bash
# From source/eDrive.OSC directory:
dotnet build eDrive.OSC/                    # Core library
dotnet build eDrive.OSC.Network/            # Network transports  
dotnet build eDrive.OSC.Tests/              # Test project
```

### Running Specific Tests
```bash
cd eDrive.OSC.Tests
dotnet test --filter "TestClassName"        # Run specific test class
dotnet test --filter "FullyQualifiedName~TestMethodName"  # Run specific test
```

### Working with Documentation
The repository contains extensive README files:
- `/README.md` - Main project overview and quick start
- `/source/eDrive.OSC/eDrive.OSC/README.md` - Core library documentation
- `/source/eDrive.OSC/eDrive.OSC.Network/README.md` - Network transport guide
- `/source/eDrive.OSC/eDrive.OSC.Reactive/README.md` - Reactive Extensions usage
- Each project has comprehensive documentation with code examples

### Repository Navigation Quick Reference
```bash
# Repository root contents:
ls -la /home/runner/work/eDrive.OSC/eDrive.OSC/
# .git .gitignore LICENSE README.md source/

# Main source directory:
ls -la source/eDrive.OSC/
# eDrive.OSC/ eDrive.OSC.Interfaces/ eDrive.OSC.Network/ 
# eDrive.OSC.Network.NamedPipes/ eDrive.OSC.Reactive/ eDrive.OSC.Tests/ eDrive.OSC.slnx

# Core project structure example:
ls -la source/eDrive.OSC/eDrive.OSC/
# README.md eDrive.OSC.csproj bin/ obj/ [source files]
```

## Development Guidelines

### Target Framework
- All projects target .NET 9.0 (`<TargetFramework>net9.0</TargetFramework>`)
- Cross-platform compatibility (.NET 9.0+)
- NamedPipes project is Windows-specific

### Dependencies
- Core: Newtonsoft.Json for JSON serialization
- Tests: xUnit v3, FluentAssertions, Microsoft.NET.Test.Sdk
- Reactive: System.Reactive 6.0+
- No additional linting or formatting tools configured

### Testing Strategy  
- Comprehensive test coverage with 193 tests
- Tests include serialization, network protocols, edge cases (NaN, Infinity)
- Test execution time: ~6 seconds for full suite
- Uses xUnit v3 with FluentAssertions for readable test code

### Code Quality
- Build produces warnings (unused variables, nullable references) but these are acceptable
- No automated linting or code formatting tools detected
- Focus on functionality and comprehensive test coverage

## Troubleshooting

### Common Issues
- **"The current .NET SDK does not support targeting .NET 9.0"**: Install .NET 9.0 SDK using the setup commands above
- **Test execution fails**: Ensure .NET 9.0 runtime is installed: `curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --runtime dotnet`
- **Build warnings**: Current warnings about unused variables and nullable references are expected and acceptable

### Performance Expectations
- Initial .NET 9.0 SDK installation: 2-3 minutes
- Full solution build: 2 seconds  
- Complete test suite: 6 seconds (193 tests)
- Individual project builds: <1 second

## Implementation Examples

### Basic OSC Message Usage
```csharp
using eDrive.OSC;

// Create message with address, then append arguments
var message = new OscMessage("/synth/freq");
message.Append(440.0f);    // float
message.Append("note");    // string
message.Append(true);      // boolean

// Or create with single argument
var simpleMessage = new OscMessage("/test", 42);

// Access message properties
Console.WriteLine($"Address: {message.Address}");
Console.WriteLine($"Data count: {message.Data.Count}");

// Binary serialization
byte[] bytes = message.ToByteArray();
```

### Network Transport Example
```csharp
using eDrive.OSC.Network.Upd;

// Send messages
using var sender = new OscOutboundUpdStream("127.0.0.1", 9000);
await sender.SendAsync(message);

// Receive messages
using var receiver = new OscInboundUdpStream(9001);  
receiver.PacketReceived += (sender, packet) => {
    if (packet is OscMessage msg)
        Console.WriteLine($"Received: {msg.Address}");
};
receiver.Start();
```

---

**Always validate your changes by running both `dotnet build` and `dotnet test` before completing any modifications.**