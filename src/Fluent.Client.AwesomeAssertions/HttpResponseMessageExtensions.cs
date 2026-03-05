// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using AwesomeAssertions.Execution;

namespace Fluent.Client.AwesomeAssertions;

public static class HttpResponseMessageExtensions
{
    extension(global::System.Net.Http.HttpResponseMessage response)
    {
        /// <summary>
        /// Returns assertion methods for an already-awaited <see cref="global::System.Net.Http.HttpResponseMessage"/>.
        /// </summary>
        /// <remarks>
        /// Use this overload when you have a materialised <see cref="global::System.Net.Http.HttpResponseMessage"/>
        /// instance. When working with Fluent.Client methods that return
        /// <see cref="Task{HttpResponseMessage}"/>, call <c>.Should()</c> on the task instead -
        /// see <c>HttpResponseMessageTaskExtensions</c> for the async-aware entry point.
        /// </remarks>
        public HttpResponseMessageAssertions Should() => new(response, AssertionChain.GetOrCreate());
    }
}
