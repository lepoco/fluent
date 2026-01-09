// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;

namespace Fluent.Client;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        /// <summary>
        /// Starts defining a fluent HTTP request with the provided body content.
        /// </summary>
        /// <param name="body">The request body.</param>
        public FluentHttpRequest With<TRequest>(TRequest body)
            where TRequest : class => new(client, new FluentHttpRequestContents { Body = body });

        /// <summary>
        /// Starts defining a fluent HTTP request with the specified authorization credentials.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The access token.</param>
        /// <param name="kind">The token kind. Defaults to "Bearer".</param>
        public FluentHttpRequest Authorize(
            string? username = null,
            string? password = null,
            string? token = null,
            AuthorizationType? kind = null
        ) => new FluentHttpRequest(client).Authorize(username, password, token, kind);

        /// <summary>
        /// Sends an HTTP POST request asynchronously.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Post(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        ) => new FluentHttpRequest(client).Post(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP POST request asynchronously and deserializes the JSON response.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Post<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class =>
            new FluentHttpRequest(client).Post<TResponse>(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP PUT request asynchronously.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Put(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        ) => new FluentHttpRequest(client).Put(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP PUT request asynchronously and deserializes the JSON response.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Put<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class =>
            new FluentHttpRequest(client).Put<TResponse>(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP PATCH request asynchronously.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Patch(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        ) => new FluentHttpRequest(client).Patch(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP PATCH request asynchronously and deserializes the JSON response.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Patch<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class =>
            new FluentHttpRequest(client).Patch<TResponse>(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP DELETE request asynchronously.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Delete(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        ) => new FluentHttpRequest(client).Delete(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP DELETE request asynchronously and deserializes the JSON response.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Delete<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class =>
            new FluentHttpRequest(client).Delete<TResponse>(path, body, cancellationToken);

        /// <summary>
        /// Sends an HTTP GET request asynchronously.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Get(
            string path = "",
            CancellationToken cancellationToken = default
        ) => new FluentHttpRequest(client).Get(path, cancellationToken);

        /// <summary>
        /// Sends an HTTP GET request asynchronously and deserializes the JSON response.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Get<TResponse>(string path = "", CancellationToken cancellationToken = default)
            where TResponse : class => new FluentHttpRequest(client).Get<TResponse>(path, cancellationToken);
    }
}
