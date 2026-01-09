// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fluent.Client;

/// <summary>
/// Represents a fluent HTTP request.
/// </summary>
/// <param name="client"><see cref="System.Net.Http.HttpClient"/> to use for sending the request.</param>
/// <param name="contents">Contents of the HTTP request.</param>
public sealed class FluentHttpRequest(System.Net.Http.HttpClient client, FluentHttpRequestContents contents)
{
    public static JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        IncludeFields = false,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Gets the contents of the Fluent HTTP request.
    /// </summary>
    public FluentHttpRequestContents Contents => contents;

    /// <summary>
    /// Sends the HTTP request asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<HttpResponseMessage> SendAsync(CancellationToken cancellationToken = default)
    {
        object[] encodedParameters =
            contents
                .QueryParameters?.Select(p => (object)Uri.EscapeDataString(p.ToString() ?? string.Empty))
                .ToArray()
            ?? [];

        HttpRequestMessage request = new()
        {
            Method = contents.HttpMethod ?? HttpMethod.Get,
            RequestUri = new Uri(
                string.Format(contents.Path ?? "/", encodedParameters),
                UriKind.RelativeOrAbsolute
            ),
        };

        request.Headers.TryAddWithoutValidation("User-Agent", contents.UserAgent);
        request.Headers.TryAddWithoutValidation("Accept", contents.AcceptedContentType);
        request.Headers.TryAddWithoutValidation("Accept-Language", contents.Culture);
        request.Headers.TryAddWithoutValidation("Lang", contents.Culture);

        foreach (KeyValuePair<string, string> header in contents.Headers ?? [])
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (contents.Body is not null)
        {
            string jsonContent = JsonSerializer.Serialize(contents.Body, DefaultJsonOptions);

#if NETFRAMEWORK
            HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, contents.ContentType);
#else
            HttpContent httpContent = new StringContent(
                jsonContent,
                Encoding.UTF8,
                new System.Net.Http.Headers.MediaTypeHeaderValue(contents.ContentType)
            );
#endif

            request.Content = httpContent;
        }

        if (contents.Timeout.HasValue)
        {
            using CancellationTokenSource cts = new(contents.Timeout.Value);
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                cts.Token
            );

            return client.SendAsync(request, linkedCts.Token);
        }

        return client.SendAsync(request, cancellationToken);
    }
}
