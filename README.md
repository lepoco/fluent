# Fluent Framework

[Created with ❤ in Poland by Leszek Pomianowski](https://lepo.co/) and [open-source community](https://github.com/lepoco/fluent/graphs/contributors).  
A collection of small, independent .NET libraries that wrap common infrastructure with a chainable API.

[![GitHub license](https://img.shields.io/github/license/lepoco/fluent)](https://github.com/lepoco/fluent/blob/main/LICENSE) [![Contributors](https://img.shields.io/github/contributors/lepoco/fluent)](https://github.com/lepoco/fluent/graphs/contributors)

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| [Fluent.Client](src/Fluent.Client) | Chainable HTTP client wrapper for .NET | [![NuGet](https://img.shields.io/nuget/v/Fluent.Client.svg)](https://www.nuget.org/packages/Fluent.Client) |
| [Fluent.Client.AwesomeAssertions](src/Fluent.Client.AwesomeAssertions) | HTTP response assertions for integration tests | [![NuGet](https://img.shields.io/nuget/v/Fluent.Client.AwesomeAssertions.svg)](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) |

## Getting started

```powershell
dotnet add package Fluent.Client
```

```csharp
using Fluent.Client;

var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };

using var response = await client
    .Authorize(token: "jwt-token")
    .Post("/api/users", new { Name = "John" });
```

Pair it with `Fluent.Client.AwesomeAssertions` for integration tests:

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
```

```csharp
using Fluent.Client.AwesomeAssertions;

await client
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because valid data was provided");
```

## Building from source

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) or later.

```powershell
git clone https://github.com/lepoco/fluent.git
cd fluent
dotnet build
dotnet test
```

## Contributing

Pull requests are welcome. See [Contributing.md](Contributing.md) for details on branching, commit conventions, and code style.

For bug reports and feature requests, open a [GitHub issue](https://github.com/lepoco/fluent/issues/new). Longer discussions belong in [GitHub Discussions](https://github.com/lepoco/fluent/discussions).

## License

Fluent Framework is free and open source software licensed under the **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
