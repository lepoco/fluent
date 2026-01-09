// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace Fluent.Client.AwesomeAssertions;

public class HttpResponseMessageTaskAssertions(Task<HttpResponseMessage> instance, AssertionChain chain)
    : ReferenceTypeAssertions<Task<HttpResponseMessage>, HttpResponseMessageTaskAssertions>(instance, chain)
{
    private AssertionChain chain = chain;

    protected override string Identifier => "http-response";

    public static JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        IncludeFields = false,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Asserts that the HTTP response indicates success (status code 2xx).
    /// </summary>
    public async Task Succeed(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(response.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to be successful, but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response has the expected status code.
    /// </summary>
    public async Task Succeed(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(response.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to have status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a failure (non-success status code).
    /// </summary>
    public async Task Fail([StringSyntax("CompositeFormat")] string because = "", params object[] becauseArgs)
    {
        using HttpResponseMessage response = await Subject;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!response.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to fail, but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response has the expected status code.
    /// </summary>
    public async Task HaveStatusCode(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(response.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to have status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );
    }

    public async Task Satisfy<TBody>(
        Action<TBody> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        using HttpResponseMessage response = await Subject;
        string? rawResponse = await response.Content.ReadAsStringAsync();

        // TODO: When 400 can deserialize problem response
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(rawResponse))
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (CurrentAssertionChain.Succeeded)
        {
            string[] failuresFromInspector;

            using (AssertionScope assertionScope = new())
            {
                TBody? body = JsonSerializer.Deserialize<TBody>(rawResponse, DefaultJsonOptions);

                assertion(body!);
                failuresFromInspector = assertionScope.Discard();
            }

            if (failuresFromInspector.Length > 0)
            {
                string failureMessage =
                    Environment.NewLine + string.Join(Environment.NewLine, failuresFromInspector);

                CurrentAssertionChain
                    .WithDefaultIdentifier(Identifier)
                    .WithExpectation(
                        "Expected {context:object} to match inspector, but the inspector was not satisfied:",
                        Subject,
                        assertionChain => assertionChain.FailWith(failureMessage)
                    );
            }
        }
    }
}
