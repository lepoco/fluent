// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

namespace Fluent.Client.AwesomeAssertions;

/// <summary>
/// A snapshot of an HTTP response passed to <c>Satisfy&lt;TBody&gt;</c> callbacks.
/// </summary>
/// <remarks>
/// Use this type when you need to assert on status code, body, and headers together inside a
/// <c>Satisfy</c> lambda without re-awaiting the response. The instance is pre-populated before
/// the callback is invoked, so all properties are safe to access synchronously.
/// </remarks>
public sealed record ReceivedHttpResponse
{
    /// <summary>
    /// The HTTP status code returned by the server.
    /// </summary>
    public required System.Net.HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// The raw response body as a string (JSON, XML, plain text, etc.).
    /// </summary>
    /// <remarks>
    /// The value is read once and cached. Check <see cref="IsEmpty"/> before attempting to
    /// deserialize or parse this value.
    /// </remarks>
    public required string Content { get; init; }

    /// <summary>
    /// Response and content headers flattened into key/value pairs, where each value array
    /// holds one or more header values for that header name.
    /// </summary>
    public required KeyValuePair<string, string[]>[] Headers { get; init; }

    /// <summary>
    /// Returns <see langword="true"/> when <see cref="Content"/> is <see langword="null"/>,
    /// empty, or consists only of whitespace.
    /// </summary>
    /// <remarks>
    /// Use as a guard before attempting to deserialize or parse the response body to avoid
    /// <see cref="System.Text.Json.JsonException"/> on empty responses.
    /// </remarks>
    public bool IsEmpty() => string.IsNullOrWhiteSpace(Content);

    /// <summary>
    /// Returns <see langword="true"/> when <see cref="StatusCode"/> is in the 200–299 range.
    /// </summary>
    /// <remarks>
    /// Useful for conditional assertions inside a <c>Satisfy</c> callback when the outcome
    /// depends on whether the request actually succeeded.
    /// </remarks>
    public bool IsSuccessStatusCode() => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);
}
