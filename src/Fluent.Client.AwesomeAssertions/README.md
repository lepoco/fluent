<div align="center">
    <h1>🧪 Fluent.Client.AwesomeAssertions</h1>
    <h3><em>Write expressive HTTP assertions for .NET integration tests.</em></h3>
</div>

<p align="center">
    <strong>A fluent assertion library for <code>Task&lt;HttpResponseMessage&gt;</code> that makes your HTTP tests readable, maintainable, and delightful to write.</strong>
</p>

<p align="center">
    <a href="https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions"><img src="https://img.shields.io/nuget/v/Fluent.Client.AwesomeAssertions.svg" alt="NuGet"/></a>
    <a href="https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions"><img src="https://img.shields.io/nuget/dt/Fluent.Client.AwesomeAssertions.svg" alt="NuGet Downloads"/></a>
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
  - [1. Install the Package](#1-install-the-package)
  - [2. (Optional) Add Fluent.Client](#2-optional-add-fluentclient)
  - [3. Start Writing Tests](#3-start-writing-tests)
- [📖 How to Use](#-how-to-use)
  - [1. Assert Success](#1-assert-success)
  - [2. Assert Specific Status Code](#2-assert-specific-status-code)
    - [Quick Reference](#quick-reference)
  - [3. Assert Failure](#3-assert-failure)
  - [4. Assert Body Content](#4-assert-body-content)
  - [5. Authorization with Query Parameters](#5-authorization-with-query-parameters)
    - [Bearer Token Authorization](#bearer-token-authorization)
    - [Basic Authentication](#basic-authentication)
    - [Authorization Methods](#authorization-methods)
- [🧪 Integration Testing](#-integration-testing)
  - [Key Patterns](#key-patterns)
- [📚 API Reference](#-api-reference)
  - [Assertion Methods](#assertion-methods)
  - [JSON Serialization](#json-serialization)
- [👥 Maintainers](#-maintainers)
- [💬 Support](#-support)
- [🙏 Acknowledgements](#-acknowledgements)
- [📄 License](#-license)

---

## 🤔 Why This Library?

Traditional HTTP testing in .NET is verbose and hard to read:

```csharp
// ❌ Traditional approach - verbose and unclear intent
var response = await client.PostAsync("/api/users", content);
Assert.True(response.IsSuccessStatusCode);
var body = await response.Content.ReadAsStringAsync();
var user = JsonSerializer.Deserialize<User>(body);
Assert.Equal("John", user.Name);
```

With **Fluent.Client.AwesomeAssertions**, your tests become expressive and self-documenting:

```csharp
// ✅ Fluent approach - clear intent, readable assertions
await client
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Satisfy<User>(u => u.Name.Should().Be("John"));
```

> [!NOTE]
> `Fluent.Client` is optional. This library works with standard `HttpClient.PostAsync()`, `GetAsync()`, etc.

---

## ⚡ Get Started

### 1. Install the Package

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
```

📦 **NuGet:** <https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions>

### 2. (Optional) Add Fluent.Client

For an even more fluent API experience:

```powershell
dotnet add package Fluent.Client
```

📦 **NuGet:** <https://www.nuget.org/packages/Fluent.Client>

> [!TIP]
> With `Fluent.Client`, you get extension methods like `.Post()`, `.Get()`, `.Delete()`, and `.Authorize()` directly on `HttpClient`.

### 3. Start Writing Tests

```csharp
using Fluent.Client;
using Fluent.Client.AwesomeAssertions;

[Fact]
public async Task CreateUser_ReturnsSuccess()
{
    await client
        .Post("/api/users", new { Name = "John" })
        .Should()
        .Succeed("because valid user data was provided");
}
```

---

## 📖 How to Use

### 1. Assert Success

Assert that a request was successful (2xx status code).

<details>
<summary><strong>Standard HttpClient</strong></summary>

```csharp
await client
    .PostAsync("/api/users", content)
    .Should()
    .Succeed();
```

</details>

<details open>
<summary><strong>With Fluent.Client</strong></summary>

```csharp
await client
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because the server returned 200 OK");
```

</details>

> [!TIP]
> The `because` parameter is optional but recommended for clearer test failure messages.

---

### 2. Assert Specific Status Code

Assert that a request returned a specific HTTP status code.

```csharp
await client
    .Delete("/api/users/123")
    .Should()
    .HaveStatusCode(HttpStatusCode.NoContent, "because delete should return 204");
```

#### Quick Reference

| Method | Description |
|--------|-------------|
| `HaveStatusCode(HttpStatusCode)` | Asserts exact status code match |
| `Succeed()` | Asserts any 2xx status code |
| `Fail()` | Asserts any non-2xx status code |

---

### 3. Assert Failure

Assert that a request failed (non-2xx status code).

```csharp
await client
    .Post("/api/basket", new { CartItem = "esp32-dev-board" })
    .Should()
    .Fail("because the server returned 400 Bad Request");
```

> [!IMPORTANT]
> `Fail()` passes for **any** non-success status code (4xx, 5xx). Use `HaveStatusCode()` if you need to verify a specific error code.

---

### 4. Assert Body Content

Assert on the deserialized response body using `Satisfy<T>`.

```csharp
await client
    .Authorize(token: "abc123")
    .Get("/api/users/1", new 
    {
        // Query parameters as anonymous object
        includeDetails = true
    })
    .Should()
    .Satisfy<User>(user =>
    {
        user.Name.Should().Be("John");
        user.Id.Should().Be(1);
    }, "because the server returned the expected JSON body");
```

<details>
<summary><strong>How it works</strong></summary>

1. Awaits the HTTP response
2. Reads the response body as string
3. Deserializes JSON to `T` using `System.Text.Json`
4. Executes your assertion lambda against the deserialized object

</details>

> [!WARNING]
> If the response body is not valid JSON or cannot be deserialized to `T`, the assertion will fail with a descriptive error message.

---

### 5. Authorization with Query Parameters

Combine authorization headers with query parameters for authenticated requests.

#### Bearer Token Authorization

```csharp
await client
    .Authorize(token: "abc123")
    .Post("/v1/api/basket")
    .Should()
    .Succeed();
```

#### Basic Authentication

```csharp
await client
    .Authorize(username: "john", password: "potato")
    .Get("/v1/api/basket", new
    {
        page = 1,
        limit = 2,
        sortBy = "dateAsc",
    })
    .Should()
    .HaveStatusCode(HttpStatusCode.Unauthorized, "because the credentials are invalid");
```

#### Authorization Methods

| Method | Header Format |
|--------|---------------|
| `.Authorize(token: "...")` | `Authorization: Bearer {token}` |
| `.Authorize(username, password)` | `Authorization: Basic {base64}` |

---

## 🧪 Integration Testing

The library excels in integration testing scenarios with multi-step workflows.

<details open>
<summary><strong>Complete Workflow Example</strong></summary>

```csharp
[Collection("Integration Tests")]
public sealed class OrderWorkflowTests(AspireAppHostFixture app)
{
    [Fact]
    public async Task Order_WhenCreatedAndProcessed_CompletesSuccessfully()
    {
        Guid orderId = Guid.NewGuid();

        // Step 1: Create resource
        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}", new { ProductId = "SKU-001", Quantity = 2 })
            .Should()
            .Succeed("because order creation should be accepted");

        // Step 2: Verify resource state
        await app.Client
            .Authorize(token: "jwt-token")
            .Get($"v1/orders/{orderId}")
            .Should()
            .Satisfy<OrderResponse>(order =>
            {
                order.Status.Should().Be("Pending");
                order.Id.Should().Be(orderId);
            });

        // Step 3: Transition state
        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}/confirm")
            .Should()
            .Succeed("because order confirmation should succeed");

        // Step 4: Complete workflow
        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}/complete", new { Note = "Delivered" })
            .Should()
            .Succeed("because order completion should succeed");
    }
}
```

</details>

### Key Patterns

| Pattern | Description |
|---------|-------------|
| 🏗️ **Test Fixtures** | Use `WebApplicationFactory`, `AspireAppHostFixture`, or similar for shared test setup |
| 🔗 **Workflow Chaining** | Chain multiple API calls to test complete business flows |
| ✅ **State Verification** | Use `Satisfy<T>` to verify intermediate states |
| 📝 **Descriptive Messages** | Add `because` messages for clear failure diagnostics |

---

## 📚 API Reference

### Assertion Methods

| Method | Description |
|--------|-------------|
| `Succeed()` | Asserts HTTP response has 2xx status code |
| `Succeed(HttpStatusCode)` | Asserts HTTP response has specific success status code |
| `Fail()` | Asserts HTTP response has non-2xx status code |
| `HaveStatusCode(HttpStatusCode)` | Asserts HTTP response has exact status code |
| `Satisfy<T>(Action<T>)` | Deserializes body and runs assertions on the result |

### JSON Serialization

<details>
<summary><code>Satisfy&lt;T&gt;</code> uses the following <code>JsonSerializerOptions</code> by default</summary>

```csharp
new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    WriteIndented = true,
    IncludeFields = false,
    Converters = { new JsonStringEnumConverter() }
}
```

</details>

---

## 👥 Maintainers

- Leszek Pomianowski ([@pomianowski](https://github.com/pomianowski))

---

## 💬 Support

For support, please open a [GitHub issue](https://github.com/lepoco/fluent/issues/new). We welcome bug reports, feature requests, and questions.

---

## 🙏 Acknowledgements

This project builds on the excellent [AwesomeAssertions](https://github.com/awesomeassertions/awesomeassertions) library and is inspired by the need for better HTTP testing ergonomics in .NET.

---

## 📄 License

This project is licensed under the terms of the **MIT** open source license. Please refer to the [LICENSE](../../LICENSE) file for the full terms.

You can use it in private and commercial projects. Keep in mind that you must include a copy of the license in your project.
