// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net;
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
}
