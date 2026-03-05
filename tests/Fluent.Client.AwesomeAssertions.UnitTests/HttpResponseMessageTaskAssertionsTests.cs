// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Fluent.Client.AwesomeAssertions.UnitTests.Stubs;
using Xunit.Sdk;

namespace Fluent.Client.AwesomeAssertions.UnitTests;

// ReSharper disable AccessToDisposedClosure
public sealed class HttpResponseMessageTaskAssertionsTests
{
    [Fact]
    public async Task HaveStatusCode_ShouldCatchSuccess_WhenGivenRequestWithQuery()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Authorize(username: "john", password: "potato")
            .Get(
                "/v1/api/basket",
                new
                {
                    page = 1,
                    limit = 2,
                    sortBy = "dateAsc",
                }
            )
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized, "because the server returned 200 OK");
    }

    [Fact]
    public async Task Succeed_ShouldCatchSuccess_WhenServerReturnsOk()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Post("/v1/api/basket").Should().Succeed("because the server returned 200 OK");
    }

    [Fact]
    public async Task Satisfy_ShouldVerifyResponseBody_WhenServerReturnsExpectedJson()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new TestResponse { Id = 42, Name = "The Answer" }),
                        new MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Authorize(token: "abc123")
            .Post("/v1/api/basket")
            .Should()
            .Satisfy<TestResponse>(
                s =>
                {
                    s.Id.Should().Be(42, "because the Id should be 42");
                    s.Name.Should().Be("The Answer", "because the Name should be 'The Answer'");
                },
                "because the server returned the expected JSON body"
            );
    }

    [Fact]
    public async Task Satisfy_ShouldVerifyResponse_WhenServerReturnsOne()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new TestResponse { Id = 42, Name = "The Answer" }),
                        new MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Authorize(token: "abc123")
            .Post("/v1/api/basket")
            .Should()
            .Satisfy(
                response =>
                {
                    response.Should().Succeed("because the server returned a successful status code");
                },
                "because the server returned a successful status code"
            );
    }

    [Fact]
    public async Task Satisfy_ShouldVerifyResponseAsynchronously_WhenServerReturnsOne()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new TestResponse { Id = 42, Name = "The Answer" }),
                        new MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Authorize(token: "abc123")
            .Post("/v1/api/basket")
            .Should()
            .Satisfy(
                async response =>
                {
                    response.Should().Succeed("because the server returned a successful status code");

                    await Task.CompletedTask;
                },
                "because the server returned a successful status code"
            );
    }

    [Fact]
    public async Task Fail_ShouldCatchFailure_WhenServerReturnsBadRequest()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Bad Request" }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Post("/v1/api/basket", new { CartItem = "esp32-dev-board" })
            .Should()
            .Fail("because the server returned 400 Bad Request");
    }

    [Fact]
    public async Task HaveStatusCode_ShouldVerifyStatusCode_WhenServerReturnsUnauthorized()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.Forbidden) { ReasonPhrase = "Unathorized" }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Delete("/humanity")
            .Should()
            .HaveStatusCode(HttpStatusCode.Forbidden, "because the server returned 403 Forbidden");
    }

    [Fact]
    public async Task Satisfy_ShouldThrowJsonException_WhenServerReturnsInvalidJson()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{invalid json content}",
                        new MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client
                .Get("/v1/api/basket")
                .Should()
                .Satisfy<TestResponse>(
                    s =>
                    {
                        s.Id.Should().Be(42, "because the Id should be 42");
                        s.Name.Should().Be("The Answer", "because the Name should be 'The Answer'");
                    },
                    "because the server returned invalid JSON body"
                );

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task HaveHeader_ShouldPass_WhenHeaderExists()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Headers = { { "X-Custom-Header", "value" } } }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/basket").Should().HaveHeader("X-Custom-Header");
    }

    [Fact]
    public async Task HaveHeader_ShouldFail_WhenHeaderDoesNotExist()
    {
        using HttpClient client = new(new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client.Get("/v1/api/basket").Should().HaveHeader("X-Custom-Header");

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task HaveHeader_WithValue_ShouldPass_WhenHeaderHasExpectedValue()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Headers = { { "X-Custom", "test-value" } } }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/basket").Should().HaveHeader("X-Custom", expectedValue: "test-value");
    }

    [Fact]
    public async Task HaveHeader_WithValue_ShouldFail_WhenHeaderHasWrongValue()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Headers = { { "X-Custom", "actual-value" } } }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client.Get("/v1/api/basket").Should().HaveHeader("X-Custom", expectedValue: "test-value");

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task HaveHeaders_ShouldPass_WhenAllHeadersExist()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Headers = { { "X-First-Header", "first" }, { "X-Second-Header", "second" } },
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/basket").Should().HaveHeaders("X-First-Header", "X-Second-Header");
    }

    [Fact]
    public async Task HaveHeaders_ShouldFail_WhenSomeHeadersAreMissing()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Headers = { { "X-First-Header", "first" } } }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client
                .Get("/v1/api/basket")
                .Should()
                .HaveHeaders("X-First-Header", "X-Second-Header", "X-Third-Header");

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task HaveHeader_ShouldPass_WhenContentHeaderExists()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("content") }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/basket").Should().HaveHeader("Content-Type");
    }

    //
    // [Fact]
    // public async Task SendAsync_ShouldSatisfyAssertion_WhenBodyIsExpected()
    // {
    //     // Arrange
    //     var responseContent = """{"id":1, "name":"Test Item"}""";
    //     var handler = new FakeHttpMessageHandler(
    //         new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseContent) }
    //     );
    //
    //     using var client = new HttpClient(handler)
    //     {
    //         BaseAddress = new Uri("https://lepo.co"),
    //     };
    //
    //     // Act & Assert
    //     await client
    //         .With(new { })
    //         .SendAsync()
    //         .Should.SucceedAsync()
    //         .And.SatisfyAsync<TestItem>(item =>
    //         {
    //             // Simple assertion - usually we would use AwesomeAssertions here too if available for the object
    //             if (item.Id != 1)
    //                 throw new Exception("Id mismatch");
    //             if (item.Name != "Test Item")
    //                 throw new Exception("Name mismatch");
    //         });
    // }
    //
    // [Fact]
    // public async Task SendAsync_ShouldRespectTimeout()
    // {
    //     // Arrange
    //     var handler = new FakeHttpMessageHandler(
    //         async (request) =>
    //         {
    //             await Task.Delay(500); // Simulate delay
    //             return new HttpResponseMessage(HttpStatusCode.OK);
    //         }
    //     );
    //
    //     using var client = new HttpClient(handler)
    //     {
    //         BaseAddress = new Uri("https://lepo.co"),
    //     };
    //
    //     var request = client.With(new { });
    //     request.Contents.Timeout = TimeSpan.FromMilliseconds(100);
    //
    //     // Act & Assert
    //     // We expect a TaskCanceledException or OperationCanceledException
    //     await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await request.SendAsync());
    // }
    //

    [Fact]
    public async Task Fail_WithStatusCode_ShouldPass_WhenStatusCodeMatches()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/missing").Should().FailWith(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Fail_WithStatusCode_ShouldFail_WhenStatusCodeDoesNotMatch()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.BadRequest))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client.Get("/v1/api/missing").Should().FailWith(HttpStatusCode.NotFound);

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task BeNotFound_ShouldPass_WhenStatusCodeIs404()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/missing").Should().BeNotFound();
    }

    [Fact]
    public async Task BeUnauthorized_ShouldPass_WhenStatusCodeIs401()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/secure").Should().BeUnauthorized();
    }

    [Fact]
    public async Task BeBadRequest_ShouldPass_WhenStatusCodeIs400()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.BadRequest))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Post("/v1/api/submit").Should().BeBadRequest();
    }

    [Fact]
    public async Task BeCreated_ShouldPass_WhenStatusCodeIs201()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Created))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Post("/v1/api/resource").Should().BeCreated();
    }

    [Fact]
    public async Task BeNoContent_ShouldPass_WhenStatusCodeIs204()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Delete("/v1/api/resource/1").Should().BeNoContent();
    }

    [Fact]
    public async Task BeForbidden_ShouldPass_WhenStatusCodeIs403()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Forbidden))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/admin").Should().BeForbidden();
    }

    [Fact]
    public async Task BeConflict_ShouldPass_WhenStatusCodeIs409()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Conflict))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Put("/v1/api/resource/1").Should().BeConflict();
    }

    [Fact]
    public async Task HaveContentType_ShouldPass_WhenContentTypeMatches()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{}",
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/data").Should().HaveContentType("application/json");
    }

    [Fact]
    public async Task HaveContentType_ShouldFail_WhenContentTypeDoesNotMatch()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{}",
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () => await client.Get("/v1/api/data").Should().HaveContentType("text/xml");

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task BeRedirect_ShouldPass_WhenStatusCodeIs3xx()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.Redirect)
                {
                    Headers = { Location = new Uri("https://lepo.co/new-location") },
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/old").Should().BeRedirect();
    }

    [Fact]
    public async Task BeRedirectedTo_ShouldPass_WhenLocationMatches()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.Redirect)
                {
                    Headers = { Location = new Uri("https://lepo.co/new-location") },
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/old").Should().BeRedirectedTo("https://lepo.co/new-location");
    }

    [Fact]
    public async Task HaveEmptyBody_ShouldPass_WhenBodyIsEmpty()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("") }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Delete("/v1/api/resource/1").Should().HaveEmptyBody();
    }

    [Fact]
    public async Task HaveNonEmptyBody_ShouldPass_WhenBodyHasContent()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\": 1}") }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Get("/v1/api/resource/1").Should().HaveNonEmptyBody();
    }

    [Fact]
    public async Task HaveEmptyBody_ShouldFail_WhenBodyHasContent()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\": 1}") }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () => await client.Get("/v1/api/resource/1").Should().HaveEmptyBody();

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task SucceedWith_ShouldPass_WhenSuccessAndBodyMatchesAssertion()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new TestResponse { Id = 99, Name = "Success" }),
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Post("/v1/api/resource")
            .Should()
            .SucceedWith<TestResponse>(s =>
            {
                s.Id.Should().Be(99);
                s.Name.Should().Be("Success");
            });
    }

    [Fact]
    public async Task SucceedWith_ShouldFail_WhenResponseIsNotSuccess()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("{}") }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        Func<Task> action = async () =>
            await client
                .Post("/v1/api/resource")
                .Should()
                .SucceedWith<TestResponse>(s =>
                {
                    s.Id.Should().Be(1);
                });

        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task AsyncSatisfy_ShouldPass_WhenAsyncAssertionSucceeds()
    {
        using HttpClient client = new(
            new FakeHttpMessageHandler(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new TestResponse { Id = 7, Name = "Async" }),
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                    ),
                }
            )
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client
            .Get("/v1/api/resource/7")
            .Should()
            .Satisfy<TestResponse>(async s =>
            {
                await Task.CompletedTask;
                s.Id.Should().Be(7);
                s.Name.Should().Be("Async");
            });
    }

    private sealed record TestResponse
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}
