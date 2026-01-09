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
}
