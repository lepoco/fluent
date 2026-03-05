// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace Fluent.Client.AwesomeAssertions;

/// <summary>
/// Provides assertion methods for an already-awaited <see cref="HttpResponseMessage"/>.
/// </summary>
/// <remarks>
/// This class is more common in unit tests where the response is already materialised.
/// For integration tests using Fluent.Client — where HTTP methods return
/// <see cref="Task{HttpResponseMessage}"/> — prefer calling <c>.Should()</c> on the task
/// directly, which gives you <see cref="HttpResponseMessageTaskAssertions"/> with richer
/// async-aware assertion methods.
/// </remarks>
public class HttpResponseMessageAssertions(HttpResponseMessage instance, AssertionChain chain)
    : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>(instance, chain)
{
    private readonly AssertionChain chain = chain;

    /// <inheritdoc />
    protected override string Identifier => "http-response";

    /// <summary>
    /// Asserts that the HTTP response indicates success (status code 2xx).
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> Succeed(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to be successful, but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }
}
