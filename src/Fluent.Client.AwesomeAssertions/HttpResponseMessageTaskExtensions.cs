// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using AwesomeAssertions.Execution;

namespace Fluent.Client.AwesomeAssertions;

public static class HttpResponseMessageTaskExtensions
{
    extension(Task<System.Net.Http.HttpResponseMessage> response)
    {
        /// <summary>
        /// Returns the assertion entry point for a <see cref="Task{HttpResponseMessage}"/> returned
        /// by any HttpClient HTTP method.
        /// </summary>
        /// <remarks>
        /// This is the primary way to assert HTTP responses in integration tests. Chain it directly
        /// on the client call without awaiting first.
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// await client.Post("/users", data).Should().BeCreated();
        /// </code>
        /// </example>
        public HttpResponseMessageTaskAssertions Should() => new(response, AssertionChain.GetOrCreate());
    }
}
