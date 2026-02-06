<div align="center">
    <h1>🌊 Fluent.Client</h1>
    <h3><em>A fluent HTTP client wrapper for .NET.</em></h3>
</div>

<p align="center">
    <strong>Build HTTP requests with a clean, chainable API. Less boilerplate, more productivity.</strong>
</p>

<p align="center">
    <a href="https://www.nuget.org/packages/Fluent.Client"><img src="https://img.shields.io/nuget/v/Fluent.Client.svg" alt="NuGet"/></a>
    <a href="https://www.nuget.org/packages/Fluent.Client"><img src="https://img.shields.io/nuget/dt/Fluent.Client.svg" alt="NuGet Downloads"/></a>
    <a href="https://github.com/lepoco/fluent/stargazers"><img src="https://img.shields.io/github/stars/lepoco/fluent?style=social" alt="GitHub stars"/></a>
    <a href="https://github.com/lepoco/fluent/blob/main/LICENSE"><img src="https://img.shields.io/github/license/lepoco/fluent" alt="License"/></a>
</p>

<p align="center">
    <a href="https://lepo.co/">Created in Poland by Leszek Pomianowski</a> and <a href="https://github.com/lepoco/fluent/graphs/contributors">open-source community</a>.
</p>

---

## Table of Contents

- [Table of Contents](#table-of-contents)
- [🤔 Why This Library?](#-why-this-library)
- [⚡ Get Started](#-get-started)
  - [Install the Package](#install-the-package)
  - [Quick Example](#quick-example)
- [📖 How to Use](#-how-to-use)
  - [1. Create a Request](#1-create-a-request)
    - [Available HTTP Methods](#available-http-methods)
  - [2. Configure the Request](#2-configure-the-request)
    - [Authorization](#authorization)
    - [Query Parameters](#query-parameters)
    - [Chaining Multiple Configurations](#chaining-multiple-configurations)
  - [3. Send the Request](#3-send-the-request)
- [📚 API Reference](#-api-reference)
  - [Request Creation Methods](#request-creation-methods)
  - [Configuration Methods](#configuration-methods)
  - [Execution Methods](#execution-methods)
- [🧪 Testing with AwesomeAssertions](#-testing-with-awesomeassertions)
- [👥 Maintainers](#-maintainers)
- [💬 Support](#-support)
- [📄 License](#-license)

---

## 🤔 Why This Library?

Traditional `HttpClient` usage requires verbose setup:

```csharp
// ❌ Traditional approach - verbose and repetitive
var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
request.Content = new StringContent(
    JsonSerializer.Serialize(new { Name = "John" }),
    Encoding.UTF8,
    "application/json");
var response = await client.SendAsync(request);
```

With **Fluent.Client**, requests become expressive one-liners:

```csharp
// ✅ Fluent approach - clean and readable
var response = await client
    .Authorize(token: "abc123")
    .Post("/api/users", new { Name = "John" })
    .SendAsync();
```

---

## ⚡ Get Started

### Install the Package

```powershell
dotnet add package Fluent.Client
```

📦 **NuGet:** <https://www.nuget.org/packages/Fluent.Client>

### Quick Example

```csharp
using Fluent.Client;

var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };

// Simple POST with JSON body
var response = await client
    .Post("/api/users", new { Name = "John Doe" })
    .SendAsync();
```

---

## 📖 How to Use

### 1. Create a Request

Start by creating a request using one of the HTTP method extensions on `HttpClient`.

```csharp
using Fluent.Client;

var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };

// POST with JSON body
var request = client.Post("/api/v1/users", new { Name = "John Doe" });

// GET with query parameters
var request = client.Get("/api/v1/users", new { page = 1, limit = 10 });

// DELETE
var request = client.Delete("/api/v1/users/897");

// PUT with body
var request = client.Put("/api/v1/users/897", new { Name = "Jane Doe" });
```

#### Available HTTP Methods

| Method | Description |
|--------|-------------|
| `.Get(path, query?)` | Create GET request with optional query parameters |
| `.Post(path, body?)` | Create POST request with optional JSON body |
| `.Put(path, body?)` | Create PUT request with optional JSON body |
| `.Delete(path)` | Create DELETE request |
| `.Patch(path, body?)` | Create PATCH request with optional JSON body |

---

### 2. Configure the Request

Chain configuration methods to add headers, authorization, and more.

#### Authorization

```csharp
// Bearer token
client.Authorize(token: "jwt-token-here").Get("/api/protected");

// Basic authentication
client.Authorize(username: "john", password: "secret").Get("/api/protected");
```

#### Query Parameters

```csharp
// Pass as anonymous object
client.Get("/api/users", new 
{ 
    page = 1, 
    limit = 10, 
    sortBy = "createdAt" 
});
```

#### Chaining Multiple Configurations

```csharp
var request = client
    .Authorize(token: "abc123")
    .Get("/api/v1/basket", new { includeItems = true });
```

---

### 3. Send the Request

Send the request and handle the response.

<details open>
<summary><strong>Get HttpResponseMessage</strong></summary>

```csharp
using HttpResponseMessage response = await request.SendAsync();

if (response.IsSuccessStatusCode)
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}
```

</details>

<details>
<summary><strong>Deserialize Response Automatically</strong></summary>

```csharp
// Automatically deserialize JSON response to typed object
UserCreatedResponse result = await request.SendAsync<UserCreatedResponse>();

Console.WriteLine($"Created user: {result.Id}");
```

</details>

<details>
<summary><strong>Direct Execution (No SendAsync)</strong></summary>

```csharp
// The request returns Task<HttpResponseMessage>, so you can await directly
using var response = await client
    .Authorize(token: "abc123")
    .Post("/api/users", new { Name = "John" });
```

</details>

---

## 📚 API Reference

### Request Creation Methods

| Method | Description |
|--------|-------------|
| `Get(path, query?)` | Create GET request |
| `Post(path, body?)` | Create POST request |
| `Put(path, body?)` | Create PUT request |
| `Delete(path)` | Create DELETE request |
| `Patch(path, body?)` | Create PATCH request |

### Configuration Methods

| Method | Description |
|--------|-------------|
| `Authorize(token)` | Add Bearer token authorization |
| `Authorize(username, password)` | Add Basic authentication |

### Execution Methods

| Method | Description |
|--------|-------------|
| `SendAsync()` | Send request and return `HttpResponseMessage` |
| `SendAsync<T>()` | Send request and deserialize response to `T` |

---

## 🧪 Testing with AwesomeAssertions

Pair this library with [Fluent.Client.AwesomeAssertions](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) for expressive test assertions:

```csharp
await client
    .Authorize(token: "abc123")
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because valid user data was provided");
```

📦 **Install:** `dotnet add package Fluent.Client.AwesomeAssertions`

---

## 👥 Maintainers

- Leszek Pomianowski ([@pomianowski](https://github.com/pomianowski))

---

## 💬 Support

For support, please open a [GitHub issue](https://github.com/lepoco/fluent/issues/new). We welcome bug reports, feature requests, and questions.

---

## 📄 License

This project is licensed under the terms of the **MIT** open source license. Please refer to the [LICENSE](../../LICENSE) file for the full terms.

You can use it in private and commercial projects. Keep in mind that you must include a copy of the license in your project.
