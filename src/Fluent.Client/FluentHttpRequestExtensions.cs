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
        /// <param name="token">The access token.</param>
        /// <param name="kind">The token kind. Defaults to "Bearer".</param>
        public FluentHttpRequest Authorize(string token, string kind = "Bearer")
        {
            request.Contents.Headers ??= new Dictionary<string, string>();
            request.Contents.Headers["Authorization"] = $"{kind} {token}";

            return request;
        }

        /// <summary>
        /// Sends the HTTP request asynchronously.
        /// </summary>
        public Task Send(CancellationToken cancellationToken = default) =>
            request.SendAsync(cancellationToken);

        /// <summary>
        /// Sends the HTTP request asynchronously and deserializes the response content to the specified type.
        /// </summary>
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
        /// Sends a GET request.
        /// </summary>
        public Task<TResponse> Get<TResponse>(string path = "", CancellationToken cancellationToken = default)
            where TResponse : class
        {
            request.Contents.Path = path;
            request.Contents.HttpMethod = HttpMethod.Get;

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }
    }
}
