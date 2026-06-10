# Fluent.Client

[Created in Poland by Leszek Pomianowski](https://lepo.co/) and [open-source community](https://github.com/lepoco/fluent/graphs/contributors).  
A chainable HTTP client wrapper for .NET that reduces the boilerplate around `HttpRequestMessage` and `HttpClient`.

[![NuGet](https://img.shields.io/nuget/v/Fluent.Client.svg)](https://www.nuget.org/packages/Fluent.Client) [![NuGet Downloads](https://img.shields.io/nuget/dt/Fluent.Client.svg)](https://www.nuget.org/packages/Fluent.Client) [![GitHub license](https://img.shields.io/github/license/lepoco/fluent)](https://github.com/lepoco/fluent/blob/main/LICENSE)

## Getting started

```powershell
dotnet add package Fluent.Client
```

<https://www.nuget.org/packages/Fluent.Client>

```csharp
using Fluent.Client;

var client = new HttpClient { BaseAddress = new Uri("https://api.example.com/") };

using var response = await client
    .Post("/api/users", new { Name = "John Doe" })
    .SendAsync();
```

## Creating requests

Use the HTTP method extensions on `HttpClient` to create a `FluentHttpRequest`:

```csharp
var request = client.Post("/api/v1/users", new { Name = "John Doe" });
var request = client.Get("/api/v1/users", new { page = 1, limit = 10 });
var request = client.Delete("/api/v1/users/897");
var request = client.Put("/api/v1/users/897", new { Name = "Jane Doe" });
```

| Method | Description |
|--------|-------------|
| `.Get(path, query?)` | GET with optional query parameters |
| `.Post(path, body?)` | POST with optional JSON body |
| `.Put(path, body?)` | PUT with optional JSON body |
| `.Delete(path)` | DELETE |
| `.Patch(path, body?)` | PATCH with optional JSON body |

## Configuring requests

```csharp
// Bearer token (default when token is provided)
client.Authorize(token: "jwt-token-here").Get("/api/protected");

// Basic authentication — credentials are Base64-encoded automatically
client.Authorize(username: "john", password: "secret").Get("/api/protected");

// API key — placed in the api-key header by default
client.Authorize(key: "my-api-key").Get("/api/protected");

// Explicit scheme override
client.Authorize(token: "token", kind: AuthorizationType.OAuth).Post("/api/resource");
client.Authorize(token: "token", kind: AuthorizationType.Negotiate).Get("/api/resource");

// Custom header name
client.Authorize(token: "token", header: "X-Auth-Token").Get("/api/protected");
```

```csharp
// Query parameters as anonymous object
client.Get("/api/users", new { page = 1, limit = 10, sortBy = "createdAt" });
```

```csharp
// Multiple configurations chained
var request = client
    .Authorize(token: "abc123")
    .Get("/api/v1/basket", new { includeItems = true });
```

Authorization schemes are defined in `Fluent.Client.Authenticate.AuthorizationType`. The scheme name is used verbatim as the header token — e.g. `Bearer`, `OAuth`, `HOBA`, `Negotiate`, `VAPID`.

| Method | Description |
|--------|-------------|
| `Authorize(token)` | `Authorization: Bearer {token}` |
| `Authorize(token, kind)` | `Authorization: {scheme} {token}` |
| `Authorize(username, password)` | `Authorization: Basic {base64(user:pass)}` |
| `Authorize(key)` | `api-key: {key}` |
| `Authorize(..., header)` | Uses `header` as the header name instead of the default |
| `WithHeader(key, value)` | Adds a custom request header |
| `WithHeaders(dictionary)` | Adds multiple headers at once |
| `WithTimeout(timespan)` | Sets the request timeout |
| `WithContentType(mediaType)` | Sets the `Content-Type` header |
| `WithAccept(mediaType)` | Sets the `Accept` header |
| `WithCulture(culture)` | Sets the `Accept-Language` header |
| `WithUserAgent(value)` | Sets the `User-Agent` header |
| `WithJsonOptions(options)` | Overrides the default `JsonSerializerOptions` |

### Windows authentication (NTLM / Kerberos)

NTLM and Kerberos are handled by `HttpClientHandler`, not by `Authorization` headers. Configure credentials on the handler before creating the client:

```csharp
using var handler = new HttpClientHandler
{
    Credentials = new NetworkCredential("user", "password", "domain"),
    // or for the current Windows user:
    // Credentials = CredentialCache.DefaultCredentials,
};
using var client = new HttpClient(handler) { BaseAddress = new Uri("https://internal.corp/") };

using var response = await client.Get("/api/data");
```

## Sending requests

```csharp
// Get HttpResponseMessage
using HttpResponseMessage response = await request.SendAsync();

if (response.IsSuccessStatusCode)
{
    var content = await response.Content.ReadAsStringAsync();
}
```

```csharp
// Deserialize response directly
UserCreatedResponse result = await request.Send<UserCreatedResponse>();
```

```csharp
// The request is awaitable — SendAsync is optional
using var response = await client
    .Authorize(token: "abc123")
    .Post("/api/users", new { Name = "John" });
```

## Testing

Pair with [Fluent.Client.AwesomeAssertions](https://www.nuget.org/packages/Fluent.Client.AwesomeAssertions) for integration tests:

```csharp
await client
    .Authorize(token: "abc123")
    .Post("/api/users", new { Name = "John" })
    .Should()
    .Succeed("because valid user data was provided");
```

## License

Fluent.Client is free and open source software licensed under the **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
