# Fluent HttpClient AwesomeAssertions for testing .NET apps

[Created in Poland by Leszek Pomianowski](https://lepo.co/) and [open-source community](https://github.com/lepoco/fluent/graphs/contributors).  
Fluent HttpClient AwesomeAssertions provides a set of fluent assertions for `Task<HttpResponseMessage>`.

> **Note**  
> `Fluent.Client` is optional. You can use this library with standard `HttpClient`.

## Getting started

You can add it to your project using .NET CLI:

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
```

## How to use

### 1. Assert success

You can assert that a request was successful (2xx status code).

**Standard HttpClient**

```csharp
await client
    .PostAsync("/api/users", content)
    .Should().Succeed();
```

**Fluent.Client**

```csharp
await client
    .Post("/api/users", new { Name = "John" })
    .Should().Succeed();
```

### 2. Assert specific status code

You can assert that a request returned a specific status code.


**With Fluent.Client**

```csharp
await client
    .Delete("/api/users/123")
    .Should().HaveStatusCode(HttpStatusCode.NoContent);
```

### 3. Assert failure

You can assert that a request failed (non-2xx status code).

**With Fluent.Client**

```csharp
await client
    .Get("/api/unknown")
    .Should().Fail();
```

### 4. Assert body content

You can assert on the deserialized body content.

**With Fluent.Client**

```csharp
await client
    .Authorize(token: "abc123")
    .Get("/api/users/1")
    .Should()
    .Satisfy<User>(user =>
    {
        user.Name.Should().Be("John");
        user.Id.Should().Be("1");
    });
```

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

Fluent HttpClient AwesomeAssertions is free and open source software licensed under MIT License. You can use it in private and commercial projects.
Keep in mind that you must include a copy of the license in your project.
