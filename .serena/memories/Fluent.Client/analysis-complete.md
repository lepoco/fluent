# Fluent.Client & Fluent.Client.AwesomeAssertions Analysis

## Executive Summary
Fluent.Client is a modern HTTP client library for .NET using C# 14 extension members. It provides:
- Fluent API for building and sending HTTP requests
- Support for all HTTP methods (GET, POST, PUT, DELETE, PATCH, HEAD, OPTIONS)
- Built-in serialization/deserialization with JSON
- Authorization helpers (Bearer, Basic, Digest, ApiKey, OAuth)
- Query parameter building and encoding

Fluent.Client.AwesomeAssertions provides assertion extensions for testing HTTP responses with AwesomeAssertions.

## Key Files & Structure
- `src/Fluent.Client/`: Core HTTP client library
  - `FluentHttpRequest.cs`: Main request class
  - `FluentHttpRequestContents.cs`: Request configuration
  - `FluentHttpRequestExtensions.cs`: HTTP method extensions (extension members)
  - `HttpClientExtensions.cs`: HttpClient extensions
  - `AuthorizationType.cs`: Authorization enum

- `src/Fluent.Client.AwesomeAssertions/`: Assertion library
  - `HttpResponseMessageTaskAssertions.cs`: Async response assertions
  - `HttpResponseMessageTaskExtensions.cs`: .Should() extension
  - `ReceivedHttpResponse.cs`: Response data structure

## HTTP Methods Implemented
All standard HTTP verbs support both non-generic (returns HttpResponseMessage) and generic versions (deserializes response to T):

### Non-generic (returns HttpResponseMessage):
- `Post(path, body?, cancellationToken?)`
- `Put(path, body?, cancellationToken?)`
- `Delete(path, body?, cancellationToken?)`
- `Patch(path, body?, cancellationToken?)`
- `Get(path, query?, cancellationToken?)`
- `Head(path, body?, cancellationToken?)`
- `Options(path, body?, cancellationToken?)`

### Generic (returns Task<T>):
- `Post<TResponse>(path, body?, cancellationToken?)`
- `Put<TResponse>(path, body?, cancellationToken?)`
- `Delete<TResponse>(path, body?, cancellationToken?)`
- `Patch<TResponse>(path, body?, cancellationToken?)`
- `Get<TResponse>(path, query?, cancellationToken?)`
- `Head<TResponse>(path, body?, cancellationToken?)`
- `Options<TResponse>(path, body?, cancellationToken?)`

## Extension Methods (FluentHttpRequestExtensions)
- `With(object body)` - Sets request body
- `Query(object query)` - Sets query parameters from object properties
- `WithParameter(string key, object? value)` - Adds single query parameter
- `Authorize(username?, password?, token?, kind?)` - Sets Authorization header
- `Send(cancellationToken?)` - Sends request, returns Task<HttpResponseMessage>
- `Send<TResponse>(cancellationToken?)` - Sends and deserializes response

## HttpClient Extensions
Methods on HttpClient that create a new FluentHttpRequest and chain methods:
- `With<TRequest>(body)` - Start with body
- `Authorize(...)` - Start with authorization
- `Query(query)` - Start with query parameters
- `WithParameter(key, value)` - Start with single parameter
- All HTTP methods (Post, Put, Delete, Patch, Get, Head, Options) with both non-generic and generic versions

## Assertion Methods (HttpResponseMessageTaskAssertions)
- `Succeed()` - Asserts 2xx status
- `Succeed(HttpStatusCode)` - Asserts specific status code
- `Fail()` - Asserts non-2xx status
- `HaveStatusCode(HttpStatusCode)` - Asserts specific status code
- `HasHeader(headerName)` - Checks header exists
- `HasHeader(headerName, expectedValue)` - Checks header and value
- `HasHeaders(params string[] headerNames)` - Checks multiple headers exist
- `Satisfy<TBody>(Action<TBody>)` - Deserializes body and runs assertions on it

## Key Implementation Details
1. **Extension Members**: Uses C# 14 `extension(T)` syntax for clean fluent API
2. **JSON Serialization**: Static `DefaultJsonOptions` with case-insensitive, trailing commas, pretty print
3. **URI Building**: Automatic URL encoding with query parameters
4. **Timeout Support**: Per-request timeout via TimeSpan
5. **Headers**: Custom headers via Dictionary<string, string>
6. **Content Negotiation**: Configurable Accept, Accept-Language, Content-Type, User-Agent
7. **Authorization**: Base64 encoding for Basic auth, direct token for Bearer

## Testing Patterns
- Uses `FakeHttpMessageHandler` for mocking HTTP responses
- Tests verify both request building and response assertions
- AwesomeAssertions tests follow AAA pattern
- Tests check headers, status codes, body deserialization, error cases

## What's NOT Implemented (Potential Gaps)
1. **Request/Response Logging**: No built-in request/response logging
2. **Retry Policies**: No automatic retry logic
3. **Circuit Breaking**: No circuit breaker support
4. **Content Types**: Only JSON supported (no XML, FormData, etc.)
5. **Request Interceptors/Middleware**: No request pipeline
6. **Streaming Responses**: No streaming support
7. **Multipart Form Data**: No form data support
8. **Custom Status Code Assertions**: Only Succeed/Fail/HaveStatusCode (no Redirect, NotFound specific assertions)
9. **Response Body Validation**: No response schema validation
10. **Cookie Management**: No explicit cookie handling shown
11. **Proxy Support**: No proxy configuration shown
12. **SSL/TLS Configuration**: Not exposed in API
13. **Default Headers**: Cannot set defaults across all requests (though HttpClient base address works)

## Code Quality Observations
- Uses `field` keyword for property backing
- Proper null handling with `is not null`
- Collection expressions for initialization
- StringSyntax attributes on message parameters
- Comprehensive XML documentation
- Both framework (.NET Framework) and modern (.NET 6+) support branches
