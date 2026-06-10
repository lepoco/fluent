// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net;
using System.Text.Json;
using Fluent.Client.Authentication;
using Fluent.Client.UnitTests.Stubs;

namespace Fluent.Client.UnitTests;

public class FluentHttpRequestTests
{
    [Fact]
    public async Task SendAsync_ShouldReturnSuccess_WhenServerReturnsOk()
    {
        using System.Net.Http.HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(
            client,
            new FluentHttpRequestContents
            {
                HttpMethod = HttpMethod.Post,
                Path = "/",
                Body = new { Hello = "World" },
            }
        );

        using HttpResponseMessage response = await request.SendAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        response.IsSuccessStatusCode.Should().BeTrue("because the server returned 200 OK");
    }

    [Fact]
    public void Query_WhenGivenAnonymousObject_ShouldGenerateValidUri()
    {
        using System.Net.Http.HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(
            client,
            new FluentHttpRequestContents
            {
                HttpMethod = HttpMethod.Post,
                Path = "/",
                Body = new { Hello = "World" },
            }
        );
        request.Query(
            new
            {
                page = 1,
                limit = 2,
                phrase = "%zażółćGęślą",
            }
        );

        request
            .Uri.ToString()
            .Should()
            .Be(
                "/?page=1&limit=2&phrase=%25zażółćGęślą",
                "because the query parameters should be correctly encoded in the URI"
            );
    }

    [Fact]
    public void Query_WhenGivenAnonymousObjectFromClient_ShouldGenerateValidUri()
    {
        using System.Net.Http.HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Query(
            new
            {
                page = 1,
                limit = 2,
                phrase = "%zażółćGęślą",
            }
        );

        request
            .Uri.ToString()
            .Should()
            .Be(
                "?page=1&limit=2&phrase=%25zażółćGęślą",
                "because the query parameters should be correctly encoded in the URI"
            );
    }

    [Fact]
    public void Query_WhenGivenDuplicate_ShouldGenerateValidUri()
    {
        using System.Net.Http.HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client
            .Query(
                new
                {
                    page = 1,
                    limit = 2,
                    phrase = "%zażółćGęślą",
                }
            )
            .WithParameter("page", 2)
            .WithParameter("page", 3);

        request
            .Uri.ToString()
            .Should()
            .Be(
                "?page=1&limit=2&phrase=%25zażółćGęślą&page=2&page=3",
                "because the query parameters should correctly handle duplicates in the URI"
            );
    }

    [Fact]
    public void Authorize_ShouldSetBearerToken_WhenTokenProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(token: "abc123");

        request.Contents.Headers!["Authorization"].Should().Be("Bearer abc123");
    }

    [Fact]
    public void Authorize_ShouldSetCustomKind_WhenKindProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(token: "abc123", kind: AuthorizationType.OAuth);

        request.Contents.Headers!["Authorization"].Should().Be("OAuth abc123");
    }

    [Fact]
    public void Authorize_ShouldSetBasicHeader_WhenUsernameAndPasswordProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(username: "john", password: "secret");

        string expected = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("john:secret"));
        request.Contents.Headers!["Authorization"].Should().Be($"Basic {expected}");
    }

    [Fact]
    public void Authorize_ShouldSetApiKeyHeader_WhenKeyProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(key: "my-api-key");

        request.Contents.Headers!["api-key"].Should().Be("my-api-key");
    }

    [Fact]
    public void Authorize_ShouldSetCustomApiKeyHeader_WhenKeyProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(key: "my-api-key", header: "weird-x-api");

        request.Contents.Headers!["weird-x-api"].Should().Be("my-api-key");
    }

    [Fact]
    public void Authorize_ShouldSetCustomHeaderName_WhenHeaderProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = client.Authorize(token: "abc123", header: "X-Auth-Token");

        request.Contents.Headers.Should().NotContainKey("Authorization");
        request.Contents.Headers!["X-Auth-Token"].Should().Be("Bearer abc123");
    }

    [Fact]
    public void Authorize_ShouldThrow_WhenNoCredentialsProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);

        Action act = () => request.Authorize();

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithHeader_ShouldSetCustomHeader_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithHeader("X-Custom", "test-value");

        request.Contents.Headers.Should().ContainKey("X-Custom");
        request.Contents.Headers!["X-Custom"].Should().Be("test-value");
    }

    [Fact]
    public void WithTimeout_ShouldSetTimeout_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithTimeout(TimeSpan.FromSeconds(30));

        request.Contents.Timeout.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void WithContentType_ShouldSetContentType_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithContentType("text/xml");

        request.Contents.ContentType.Should().Be("text/xml");
    }

    [Fact]
    public void WithCulture_ShouldSetCulture_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithCulture("pl-PL");

        request.Contents.Culture.Should().Be("pl-PL");
    }

    [Fact]
    public void WithAccept_ShouldSetAcceptedContentType_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithAccept("text/html");

        request.Contents.AcceptedContentType.Should().Be("text/html");
    }

    [Fact]
    public void WithUserAgent_ShouldSetUserAgent_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithUserAgent("TestBot/1.0");

        request.Contents.UserAgent.Should().Be("TestBot/1.0");
    }

    [Fact]
    public async Task Head_ShouldSendHeadRequest_WhenCalledViaHttpClient()
    {
        HttpMethod? capturedMethod = null;
        using HttpClient client = new(
            new FakeHttpMessageHandler(request =>
            {
                capturedMethod = request.Method;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            })
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        using HttpResponseMessage response = await client.Head("/v1/api/health");

        capturedMethod.Should().Be(HttpMethod.Head);
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Options_ShouldSendOptionsRequest_WhenCalledViaHttpClient()
    {
        HttpMethod? capturedMethod = null;
        using HttpClient client = new(
            new FakeHttpMessageHandler(request =>
            {
                capturedMethod = request.Method;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            })
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        using HttpResponseMessage response = await client.Options("/v1/api/cors");

        capturedMethod.Should().Be(HttpMethod.Options);
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public void WithHeaders_ShouldSetMultipleHeaders_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithHeaders(new Dictionary<string, string> { ["X-First"] = "one", ["X-Second"] = "two" });

        request.Contents.Headers.Should().ContainKey("X-First");
        request.Contents.Headers!["X-First"].Should().Be("one");
        request.Contents.Headers.Should().ContainKey("X-Second");
        request.Contents.Headers!["X-Second"].Should().Be("two");
    }

    [Fact]
    public void WithHeaders_ShouldMergeHeaders_WhenHeadersAlreadyExist()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithHeader("X-Existing", "original");
        request.WithHeaders(new Dictionary<string, string> { ["X-New"] = "added" });

        request.Contents.Headers.Should().ContainKey("X-Existing");
        request.Contents.Headers!["X-Existing"].Should().Be("original");
        request.Contents.Headers.Should().ContainKey("X-New");
        request.Contents.Headers!["X-New"].Should().Be("added");
    }

    [Fact]
    public void WithHeaders_ShouldOverrideExistingHeader_WhenSameKeyProvided()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new(client);
        request.WithHeader("X-Custom", "old-value");
        request.WithHeaders(new Dictionary<string, string> { ["X-Custom"] = "new-value" });

        request.Contents.Headers!["X-Custom"].Should().Be("new-value");
    }

    [Fact]
    public void WithHeaders_ShouldSupportChaining_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new FluentHttpRequest(client)
            .WithHeaders(new Dictionary<string, string> { ["X-First"] = "one" })
            .WithHeaders(new Dictionary<string, string> { ["X-Second"] = "two" })
            .WithTimeout(TimeSpan.FromSeconds(5));

        request.Contents.Headers.Should().ContainKey("X-First");
        request.Contents.Headers.Should().ContainKey("X-Second");
        request.Contents.Timeout.Should().Be(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void WithHeader_ShouldSupportChaining_WhenMultipleHeadersAdded()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        FluentHttpRequest request = new FluentHttpRequest(client)
            .WithHeader("X-First", "one")
            .WithHeader("X-Second", "two")
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithContentType("text/plain");

        request.Contents.Headers.Should().ContainKey("X-First");
        request.Contents.Headers.Should().ContainKey("X-Second");
        request.Contents.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        request.Contents.ContentType.Should().Be("text/plain");
    }

    [Fact]
    public void WithJsonOptions_ShouldSetCustomOptions_WhenCalled()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
        };

        FluentHttpRequest request = new(client);
        request.WithJsonOptions(customOptions);

        request.Contents.JsonOptions.Should().BeSameAs(customOptions);
    }

    [Fact]
    public async Task WithJsonOptions_ShouldUseCustomOptionsForSerialization_WhenBodyIsSet()
    {
        string? capturedBody = null;
        using HttpClient client = new(
            new FakeHttpMessageHandler(async req =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK);
            })
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
        };

        FluentHttpRequest request = new(client);
        request.WithJsonOptions(customOptions);
        request.Contents.Path = "/";
        request.Contents.HttpMethod = HttpMethod.Post;
        request.Contents.Body = new { HelloWorld = "test" };

        await request.SendAsync(cancellationToken: TestContext.Current.CancellationToken);

        capturedBody
            .Should()
            .Contain("hello_world", "because SnakeCaseLower naming policy should be applied");
    }

    [Fact]
    public async Task WithJsonOptions_ShouldUseCustomOptionsForDeserialization_WhenResponseIsReceived()
    {
        const string responseJson = """{"hello_world":"test"}""";
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json"),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        var customOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        FluentHttpRequest request = new(client);
        request.WithJsonOptions(customOptions);
        request.Contents.Path = "/";
        request.Contents.HttpMethod = HttpMethod.Get;

        WithJsonOptionsDeserializationResponse result =
            await request.Send<WithJsonOptionsDeserializationResponse>(
                cancellationToken: TestContext.Current.CancellationToken
            );

        result.HelloWorld.Should().Be("test");
    }

    private sealed record WithJsonOptionsDeserializationResponse(string HelloWorld);
}
