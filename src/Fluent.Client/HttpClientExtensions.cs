﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using HttpMethod = System.Net.Http.HttpMethod;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;

namespace Fluent.Client;

public static class HttpClientExtensions
{
    extension(System.Net.Http.HttpClient client)
    {
        /// <summary>
        /// Creates a <see cref="FluentHttpRequest"/> for the given <see cref="HttpClient"/> and attaches the given body to the request.
        /// </summary>
        public FluentHttpRequest With<TRequest>(TRequest body)
            where TRequest : class => new(client, new FluentHttpRequestContents { Body = body });

        /// <summary>
        /// Initializes a new <see cref="FluentHttpRequest"/> with an Authorization header.
        /// </summary>
        public FluentHttpRequest Authorize(string token, string kind = "Bearer") =>
            new FluentHttpRequest(client, new FluentHttpRequestContents()).Authorize(token, kind);

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        public Task<HttpResponseMessage> Post(
            string path = "",
            object? body = null,
            CancellationToken cancellationToken = default
        )
        {
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Post,
                    Path = path,
                    Body = body,
                }
            );

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
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Post,
                    Path = path,
                    Body = body,
                }
            );

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
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Put,
                    Path = path,
                    Body = body,
                }
            );

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
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Put,
                    Path = path,
                    Body = body,
                }
            );

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
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Delete,
                    Path = path,
                    Body = body,
                }
            );

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
            FluentHttpRequest request = new(
                client,
                new FluentHttpRequestContents
                {
                    HttpMethod = HttpMethod.Delete,
                    Path = path,
                    Body = body,
                }
            );

            return request.Send<TResponse>(cancellationToken: cancellationToken);
        }
    }
}
