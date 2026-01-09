# Fluent Client for .NET HttpClient.

[Created in Poland by Leszek Pomianowski](https://lepo.co/) and [open-source community](https://github.com/lepoco/fluent/graphs/contributors).  
Fluent Client provides a way to build HTTP requests. It acts as a wrapper around the standard HttpClient, allowing you to set up your requests with a body, headers, queries, and other parameters before sending them.

## Getting started

You can add it to your project using .NET CLI:

```powershell
dotnet add package Fluent.Client
```

## How to use

### 1. Create a request

You can start by creating a request with a body using the `With` method on your HttpClient.

```csharp
using Fluent.HttpClient;

var client = new HttpClient();
client.BaseAddress = new Uri("https://api.example.com/");

var request = client.Post("/api/v1/users", new { Name = "John Doe" });
```

### 2. Configure the request

You can configure its properties like the path, HTTP method, and headers.

```csharp
client.Authorize(token: "123").Delete("/api/v1/users/897");
```

### 3. Send the request

You can send the request and get the response message, or automatically deserialize the response content.

```csharp
// Send and get the HttpResponseMessage
using HttpResponseMessage response = await request.SendAsync();

// or send and deserialize the response
UserCreatedResponse result = await request.SendAsync<UserCreatedResponse>();
```

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

Fluent HttpClient is free and open source software licensed under MIT License. You can use it in private and commercial projects.
Keep in mind that you must include a copy of the license in your project.
