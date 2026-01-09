// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Fluent.Client.AwesomeAssertions.UnitTests.Stubs;

namespace Fluent.Client.AwesomeAssertions.UnitTests;

public sealed class HttpResponseMessageTaskAssertionsTests
{
    [Fact]
    public async Task Succeed_ShouldCatchSuccess_WhenServerReturnsOk()
    {
        using System.Net.Http.HttpClient client = new(
            new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK))
        )
        {
            BaseAddress = new Uri("https://lepo.co"),
        };

        await client.Post("/v1/api/basket").Should().Succeed("because the server returned 200 OK");
    }

    [Fact]
    public async Task Satisfy_ShouldVerifyResponseBody_WhenServerReturnsExpectedJson()
    {
        using System.Net.Http.HttpClient client = new(
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
    public async Task Fail_ShouldCatchFailure_WhenServerReturnsBadRequest()
    {
        using System.Net.Http.HttpClient client = new(
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
        using System.Net.Http.HttpClient client = new(
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
    //     using var client = new System.Net.Http.HttpClient(handler)
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
    //     using var client = new System.Net.Http.HttpClient(handler)
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
    private sealed record TestResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
