// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;

namespace Fluent.Client;

/// <summary>
/// Represents the contents and configuration for a fluent HTTP request.
/// This class is used to build and customize the HTTP request before sending it.
/// <para>
/// When writing tests, you can inspect instances of this class to verify that the correct
/// headers, body, parameters, and other settings were configured before the request is sent.
/// This allows for detailed assertions on the request state without needing to mock the internal
/// logic of the HTTP client.
/// </para>
/// </summary>
public class FluentHttpRequestContents
{
    /// <summary>
    /// Gets or sets the HTTP method (GET, POST, PUT, DELETE, etc.) for the request.
    /// </summary>
    public HttpMethod? HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the relative or absolute path for the request URL.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the body of the request, which will be serialized to JSON.
    /// </summary>
    public object? Body { get; set; }

    /// <summary>
    /// Gets or sets specific HTTP headers to be added to the request.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets query string parameters to be appended to the request URL.
    /// </summary>
    public ICollection<KeyValuePair<string, string?>>? QueryParameters { get; set; }

    /// <summary>
    /// Gets or sets the culture for the request, typically used for the <c>"Accept-Language"</c> header.
    /// <para>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Accept-Language"/>
    /// </para>
    /// </summary>
    public string Culture { get; set; } = "en,en-GB;q=0.9,en-US";

    /// <summary>
    /// Gets or sets the <c>"Accept"</c> header value. Defaults to "application/json".
    /// <para>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Accept"/>
    /// </para>
    /// </summary>
    public string AcceptedContentType { get; set; } = "application/json";

    /// <summary>
    /// Gets or sets the <c>"User-Agent"</c> header value.
    /// <para>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/User-Agent"/>
    /// </para>
    /// </summary>
    public string UserAgent { get; set; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36 Edg/143.0.0.0 Lepo.FluentHttpRequest/1.0";

    /// <summary>
    /// Gets or sets the <c>"Content-Type"</c> header value for the request body. Defaults to <c>"application/json;charset=utf-8"</c>.
    /// <para>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Type"/>
    /// </para>
    /// </summary>
    public string ContentType { get; set; } = "application/json";

    /// <summary>
    /// Gets or sets the timeout for the HTTP request. If null, the default client timeout is used.
    /// </summary>
    public TimeSpan? Timeout { get; set; }
}
