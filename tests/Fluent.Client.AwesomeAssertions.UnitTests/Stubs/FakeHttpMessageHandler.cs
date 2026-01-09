// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

namespace Fluent.Client.AwesomeAssertions.UnitTests.Stubs;

public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _handler = _ => Task.FromResult(response);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        return _handler(request);
    }
}
