# gRPC with .NET

A stock-ticker playground demonstrating several .NET gRPC client patterns against one service.

## Projects

| Project | Purpose |
| --- | --- |
| `Service` | Hosts the Greeter and stock-price services. |
| `ClientA`, `ClientB` | Exercise simple Greeter calls. |
| `ClientC`, `ClientD` | Consume the stock-price stream. |
| `ClientLib` | Provides shared client behavior. |

Protocol definitions are stored in `Protos/`.

## Prerequisites

- A .NET 10 SDK
- Visual Studio 2026, VS Code with C# Dev Kit, or another .NET-compatible editor

## Run the playground

Restore and build the root solution:

```pwsh
dotnet restore .\grpc-dotnet.slnx
dotnet build .\grpc-dotnet.slnx --no-restore
```

Start `Service` before starting any client. In Visual Studio, configure multiple startup projects in
this order: `Service`, `ClientA`, `ClientB`, `ClientC`, `ClientD`.

This repository contains demonstration code and does not deploy a hosted service.
