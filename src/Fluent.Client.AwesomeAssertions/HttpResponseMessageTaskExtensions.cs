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
        /// Returns assertions for <see cref="HttpResponseMessage"/>.
        /// </summary>
        public HttpResponseMessageTaskAssertions Should() => new(response, AssertionChain.GetOrCreate());
    }
}
