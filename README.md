<div align="center">
    <h1>🚀 Fluent Framework</h1>
    <h3><em>A modern collection of libraries for .NET applications.</em></h3>
</div>

<p align="center">
    <strong>Build better .NET applications with fluent, expressive APIs across HTTP, UI, and more.</strong>
</p>

<p align="center">
    <a href="https://github.com/lepoco/fluent/stargazers"><img src="https://img.shields.io/github/stars/lepoco/fluent?style=social" alt="GitHub stars"/></a>
    <a href="https://github.com/lepoco/fluent/blob/main/LICENSE"><img src="https://img.shields.io/github/license/lepoco/fluent" alt="License"/></a>
    <a href="https://github.com/lepoco/fluent/graphs/contributors"><img src="https://img.shields.io/github/contributors/lepoco/fluent" alt="Contributors"/></a>
</p>

<p align="center">
    <a href="https://lepo.co/">Created with ❤️ in Poland by Leszek Pomianowski</a> and <a href="https://github.com/lepoco/fluent/graphs/contributors">open-source community</a>.
</p>

---

## 📦 Packages

Fluent Framework is a collection of independent packages. Use what you need.

| Package | Description | NuGet |
|---------|-------------|-------|
| [**Fluent.Client**](src/Fluent.Client) | Fluent HTTP client wrapper for clean, chainable requests | [![NuGet](https://img.shields.io/nuget/v/Fluent.Client.svg)](https://www.nuget.org/packages/Fluent.Client) |
| [**Fluent.Client.AwesomeAssertions**](src/Fluent.Client.AwesomeAssertions) | Expressive HTTP response assertions for integration tests | [![NuGet](https://img.shields.io/nuget/v/Fluent.Client.AwesomeAssertions.svg)](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) |

> [!NOTE]
> More packages coming soon. Stay tuned!

---

## ⚡ Quick Start

### HTTP Client

```powershell
dotnet add package Fluent.Client
```

```csharp
using Fluent.Client;

var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };

// Clean, fluent HTTP requests
var response = await client
    .Authorize(token: "jwt-token")
    .Post("/api/users", new { Name = "John" });
```

### Testing Assertions

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
```

```csharp
using Fluent.Client.AwesomeAssertions;

// Expressive test assertions
await client
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because valid data was provided");
```

---

## 🎯 Philosophy

Fluent Framework follows these principles:

| Principle | Description |
|-----------|-------------|
| **🧩 Modular** | Use only what you need. Each package is independent. |
| **📖 Readable** | APIs designed to read like natural language. |
| **🔧 Extensible** | Easy to extend and customize for your needs. |
| **✅ Testable** | Built with testing in mind from the ground up. |
| **🚀 Modern** | Targets latest .NET with modern C# features. |

---

## 🛠️ Building from Source

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Visual Studio 2022, VS Code, or JetBrains Rider

### Build

```powershell
git clone https://github.com/lepoco/fluent.git
cd fluent
dotnet build
```

### Run Tests

```powershell
dotnet test
```

---

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](Contributing.md) for details.

### Ways to Contribute

- 🐛 Report bugs and issues
- 💡 Suggest new features or packages
- 📝 Improve documentation
- 🔧 Submit pull requests

---

## 👥 Maintainers

- Leszek Pomianowski ([@pomianowski](https://github.com/pomianowski))

---

## 💬 Support

- 📖 [Documentation](docs/)
- 🐛 [Issue Tracker](https://github.com/lepoco/fluent/issues)
- 💬 [Discussions](https://github.com/lepoco/fluent/discussions)

---

## 📄 License

This project is licensed under the **MIT** license. See the [LICENSE](LICENSE) file for details.

You can use it in private and commercial projects. Keep in mind that you must include a copy of the license in your project.
