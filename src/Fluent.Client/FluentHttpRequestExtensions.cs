// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using System.Text.Json;

namespace Fluent.Client;

public static class FluentHttpRequestExtensions
{
    extension(FluentHttpRequest request)
    {
        /// <summary>
        /// Adds an Authorization header to the request.
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
        )
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                byte[] byteArray = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");

                token = Convert.ToBase64String(byteArray);
                kind ??= AuthorizationType.Basic;
            }
            else if (!string.IsNullOrEmpty(token))
            {
                kind ??= AuthorizationType.Bearer;
            }
            else
            {
                throw new ArgumentException("Either token or username and password must be provided.");
            }

            request.Contents.Headers ??= [];
            request.Contents.Headers["Authorization"] = $"{kind} {token}";

            return request;
        }

        /// <summary>
        /// Sends the HTTP request asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Send(CancellationToken cancellationToken = default) =>
            request.SendAsync(cancellationToken);

        /// <summary>
        /// Sends the HTTP request asynchronously and deserializes the response content to the specified type.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<TResponse> Send<TResponse>(CancellationToken cancellationToken = default)
            where TResponse : class
        {
            using HttpResponseMessage response = await request.SendAsync(cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync();
            TResponse? result = JsonSerializer.Deserialize<TResponse>(
                responseContent,
                FluentHttpRequest.DefaultJsonOptions
            );

            if (result is null)
            {
                throw new InvalidOperationException("Failed to deserialize the response content.");
            }

            return result;
        }

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Post(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Post;

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Post<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Post;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Put(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Put;

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Put<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Put;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Delete(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Delete;

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Delete<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Delete;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a Options request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Options(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Options;

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a Options request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Options<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Options;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a Head request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Head(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Head;

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a Head request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Head<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = HttpMethod.Head;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Get(string path = "", CancellationToken cancellationToken = default)
        {
            request.Contents.Path = path;
            request.Contents.HttpMethod = HttpMethod.Get;

            return request.Send(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Get<TResponse>(string path = "", CancellationToken cancellationToken = default)
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.HttpMethod = HttpMethod.Get;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sends a PATCH request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<HttpResponseMessage> Patch(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            // TODO: Consider writing custom json patch impl
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = new HttpMethod("PATCH");

            return request.SendAsync(cancellationToken);
        }

        /// <summary>
        /// Sends a PATCH request.
        /// </summary>
        /// <param name="path">The request path.</param>
        /// <param name="body">The request body.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<TResponse> Patch<TResponse>(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.Body = body;
            request.Contents.HttpMethod = new HttpMethod("PATCH");

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }
    }
}
