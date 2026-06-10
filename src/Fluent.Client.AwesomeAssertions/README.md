# Fluent.Client.AwesomeAssertions

[Created in Poland by Leszek Pomianowski](https://lepo.co/) and [open-source community](https://github.com/lepoco/fluent/graphs/contributors).  
Assertion extensions for `HttpResponseMessage` and `Task<HttpResponseMessage>` built on top of [AwesomeAssertions](https://github.com/awesomeassertions/awesomeassertions).

[![NuGet](https://img.shields.io/nuget/v/Fluent.Client.AwesomeAssertions.svg)](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) [![NuGet Downloads](https://img.shields.io/nuget/dt/Fluent.Client.AwesomeAssertions.svg)](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) [![GitHub license](https://img.shields.io/github/license/lepoco/fluent)](https://github.com/lepoco/fluent/blob/main/LICENSE)

## Getting started

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
```

<https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions>

`Fluent.Client` is optional. This library works with standard `HttpClient.PostAsync()`, `GetAsync()`, etc.

```powershell
dotnet add package Fluent.Client
```

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

## Asserting responses

**Assert 2xx:**

```csharp
await client
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because the server returned 200 OK");
```

**Assert specific status code:**

```csharp
await client
    .Delete("/api/users/123")
    .Should()
    .HaveStatusCode(HttpStatusCode.NoContent, "because delete should return 204");
```

**Assert failure (any non-2xx):**

```csharp
await client
    .Post("/api/basket", new { CartItem = "esp32-dev-board" })
    .Should()
    .Fail("because the server returned 400 Bad Request");
```

**Assert on the response body:**

```csharp
await client
    .Authorize(token: "abc123")
    .Get("/api/users/1", new { includeDetails = true })
    .Should()
    .Satisfy<User>(user =>
    {
        user.Name.Should().Be("John");
        user.Id.Should().Be(1);
    }, "because the server returned the expected JSON body");
```

## Authorization

```csharp
// Bearer token
await client
    .Authorize(token: "abc123")
    .Post("/v1/api/basket")
    .Should()
    .Succeed();

// Basic authentication
await client
    .Authorize(username: "john", password: "potato")
    .Get("/v1/api/basket", new { page = 1, limit = 2, sortBy = "dateAsc" })
    .Should()
    .HaveStatusCode(HttpStatusCode.Unauthorized, "because the credentials are invalid");
```

| Method | Header |
|--------|--------|
| `.Authorize(token: "...")` | `Authorization: Bearer {token}` |
| `.Authorize(token: "...", kind: AuthorizationType.OAuth)` | `Authorization: OAuth {token}` |
| `.Authorize(username, password)` | `Authorization: Basic {base64(user:pass)}` |
| `.Authorize(key: "...")` | `api-key: {key}` |
| `.Authorize(..., header: "X-Auth-Token")` | `X-Auth-Token: {value}` |

## Integration testing

```csharp
[Collection("Integration Tests")]
public sealed class OrderWorkflowTests(AspireAppHostFixture app)
{
    [Fact]
    public async Task Order_WhenCreatedAndProcessed_CompletesSuccessfully()
    {
        Guid orderId = Guid.NewGuid();

        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}", new { ProductId = "SKU-001", Quantity = 2 })
            .Should()
            .Succeed("because order creation should be accepted");

        await app.Client
            .Authorize(token: "jwt-token")
            .Get($"v1/orders/{orderId}")
            .Should()
            .Satisfy<OrderResponse>(order =>
            {
                order.Status.Should().Be("Pending");
                order.Id.Should().Be(orderId);
            });

        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}/confirm")
            .Should()
            .Succeed("because order confirmation should succeed");

        await app.Client
            .Authorize(token: "jwt-token")
            .Put($"v1/orders/{orderId}/complete", new { Note = "Delivered" })
            .Should()
            .Succeed("because order completion should succeed");
    }
}
```

## API reference

| Method | Description |
|--------|-------------|
| `Succeed()` | Asserts 2xx status code |
| `Succeed(HttpStatusCode)` | Asserts specific success status code |
| `Fail()` | Asserts non-2xx status code |
| `HaveStatusCode(HttpStatusCode)` | Asserts exact status code |
| `Satisfy<T>(Action<T>)` | Deserializes body to `T` and runs assertions |

`Satisfy<T>` uses the following `JsonSerializerOptions` by default:

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

## License

Fluent.Client.AwesomeAssertions is free and open source software licensed under the **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
