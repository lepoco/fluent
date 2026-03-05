// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
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
public class HttpResponseMessageAssertions(HttpResponseMessage instance, AssertionChain assertionChain)
    : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>(instance, assertionChain)
{
    private readonly AssertionChain chain = assertionChain;

    /// <inheritdoc />
    protected override string Identifier => "http-response";

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    /// <summary>
    /// Shared <see cref="JsonSerializerOptions"/> used when deserializing response bodies in
    /// <c>Satisfy&lt;TBody&gt;</c> and <c>SucceedWith&lt;TBody&gt;</c>.
    /// </summary>
    /// <remarks>
    /// Default settings: property names are matched case-insensitively, trailing commas are
    /// allowed, and enums are serialized as strings. Replace this field globally to customise
    /// deserialization for your entire test suite.
    /// </remarks>
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

    /// <summary>
    /// Asserts that the HTTP response is both successful (status code 2xx) and has exactly
    /// <paramref name="expectedStatusCode"/>.
    /// </summary>
    /// <param name="expectedStatusCode">The exact status code the response must have.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> SucceedWith(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to have status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a failure (non-success status code).
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> Fail(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!Subject.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to fail, but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response has exactly <paramref name="expectedStatusCode"/>.
    /// </summary>
    /// <param name="expectedStatusCode">The exact status code the response must have.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveStatusCode(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to have status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response contains the expected header.
    /// </summary>
    /// <param name="headerName">The name of the header that must be present.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveHeader(
        string headerName,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(headerName, nameof(headerName));

        bool headerExists = ContainsHeader(Subject, headerName);

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(headerExists)
            .FailWith($"Expected HTTP response to contain header \"{headerName}\", but it was not found.");

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response contains the expected header with the expected value.
    /// </summary>
    /// <param name="headerName">The name of the header that must be present.</param>
    /// <param name="expectedValue">The value the header must contain.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveHeaderWithValue(
        string headerName,
        string expectedValue,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(headerName, nameof(headerName));
        ThrowIfNull(expectedValue, nameof(expectedValue));

        bool headerExists = ContainsHeader(Subject, headerName);

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(headerExists)
            .FailWith($"Expected HTTP response to contain header \"{headerName}\", but it was not found.");

        if (!headerExists)
        {
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        string[] actualHeaderValues = GetHeaderValues(Subject, headerName);
        string actualValues = string.Join(", ", actualHeaderValues);
        bool containsExpectedValue = actualHeaderValues.Any(value =>
            string.Equals(value, expectedValue, StringComparison.OrdinalIgnoreCase)
        );

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(containsExpectedValue)
            .FailWith(
                $"Expected HTTP response header \"{headerName}\" to contain value \"{expectedValue}\", but found \"{actualValues}\"."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response contains all expected headers.
    /// </summary>
    /// <param name="headerNames">The names of all headers that must be present.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveHeaders(params string[] headerNames)
    {
        ThrowIfNull(headerNames, nameof(headerNames));

        foreach (string headerName in headerNames)
        {
            ThrowIfNull(headerName, nameof(headerName));
        }

        string[] missingHeaders = [.. headerNames.Where(name => !ContainsHeader(Subject, name))];
        string expectedList = string.Join(", ", headerNames.Select(name => $"\"{name}\""));
        string missingList = string.Join(", ", missingHeaders.Select(name => $"\"{name}\""));

        chain
            .ForCondition(missingHeaders.Length == 0)
            .FailWith(
                $"Expected HTTP response to contain headers {expectedList}, but the following were not found: {missingList}."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a failure with the specific status code.
    /// </summary>
    /// <param name="expectedStatusCode">The exact failure status code the response must have.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> FailWith(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!Subject.IsSuccessStatusCode && Subject.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to fail with status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response has a 404 Not Found status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeNotFound(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.NotFound, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 401 Unauthorized status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeUnauthorized(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.Unauthorized, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 400 Bad Request status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeBadRequest(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.BadRequest, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 201 Created status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeCreated(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.Created, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 204 No Content status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeNoContent(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.NoContent, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 403 Forbidden status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeForbidden(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.Forbidden, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 409 Conflict status code.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeConflict(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => HaveStatusCode(System.Net.HttpStatusCode.Conflict, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has the expected Content-Type header.
    /// </summary>
    /// <param name="expectedMediaType">The media type the Content-Type header must match.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveContentType(
        string expectedMediaType,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(expectedMediaType, nameof(expectedMediaType));

        string? actualContentType = Subject.Content?.Headers?.ContentType?.MediaType;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(
                string.Equals(actualContentType, expectedMediaType, StringComparison.OrdinalIgnoreCase)
            )
            .FailWith(
                $"Expected HTTP response to have Content-Type \"{expectedMediaType}\", but found \"{actualContentType ?? "(none)"}\"."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a redirect (3xx status code).
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeRedirect(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        int statusCode = (int)Subject.StatusCode;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(statusCode >= 300 && statusCode <= 399)
            .FailWith(
                $"Expected HTTP response to be a redirect (3xx), but found {statusCode} ({Subject.ReasonPhrase})."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response is a redirect (3xx) whose <c>Location</c> header equals
    /// <paramref name="expectedUrl"/>.
    /// </summary>
    /// <param name="expectedUrl">The URL the <c>Location</c> header must match.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> BeRedirectedTo(
        string expectedUrl,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(expectedUrl, nameof(expectedUrl));

        int statusCode = (int)Subject.StatusCode;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(statusCode >= 300 && statusCode <= 399)
            .FailWith(
                $"Expected HTTP response to be a redirect (3xx), but found {statusCode} ({Subject.ReasonPhrase})."
            );

        if (statusCode < 300 || statusCode > 399)
        {
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        string? actualLocation = Subject.Headers.Location?.ToString();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(string.Equals(actualLocation, expectedUrl, StringComparison.OrdinalIgnoreCase))
            .FailWith(
                $"Expected HTTP response to redirect to \"{expectedUrl}\", but found \"{actualLocation ?? "(none)"}\"."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response has an empty body.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveEmptyBody(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        string content = Subject.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(string.IsNullOrWhiteSpace(content))
            .FailWith(
                $"Expected HTTP response to have an empty body, but found content with length {content.Length}."
            );

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response has a non-empty body.
    /// </summary>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> HaveNonEmptyBody(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        string content = Subject.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(content))
            .FailWith("Expected HTTP response to have a non-empty body, but the body was empty.");

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Asserts that the HTTP response is successful (2xx) and that the response body, deserialized
    /// to <typeparamref name="TBody"/>, satisfies the provided <paramref name="assertion"/>.
    /// </summary>
    /// <typeparam name="TBody">The type to deserialize the JSON response body into.</typeparam>
    /// <param name="assertion">A callback that receives the deserialized body and performs further assertions.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> SucceedWith<TBody>(
        Action<TBody> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to be successful, but found {(int)Subject.StatusCode} ({Subject.ReasonPhrase})."
            );

        if (!CurrentAssertionChain.Succeeded)
        {
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        string content = Subject.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(content))
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (!CurrentAssertionChain.Succeeded)
        {
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        string[] failuresFromInspector;

        using (AssertionScope assertionScope = new())
        {
            TBody? body;

            try
            {
                body = JsonSerializer.Deserialize<TBody>(content, DefaultJsonOptions);
            }
            catch (Exception e)
            {
                CurrentAssertionChain
                    .WithDefaultIdentifier(Identifier)
                    .WithExpectation(
                        $"Expected HTTP response content to be deserializable to \"{typeof(TBody).Name}\", but deserialization threw an exception:",
                        assertionChain => assertionChain.FailWith(e.ToString())
                    );

                return new AndConstraint<HttpResponseMessageAssertions>(this);
            }

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

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Deserializes the response body to <typeparamref name="TBody"/> regardless of the status
    /// code, then runs <paramref name="assertion"/> on the result.
    /// </summary>
    /// <typeparam name="TBody">The type to deserialize the JSON response body into.</typeparam>
    /// <param name="assertion">A callback that receives the deserialized body and performs further assertions.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> Satisfy<TBody>(
        Action<TBody> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        string content = Subject.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(content))
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (!CurrentAssertionChain.Succeeded)
        {
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        string[] failuresFromInspector;

        using (AssertionScope assertionScope = new())
        {
            TBody? body;

            try
            {
                body = JsonSerializer.Deserialize<TBody>(content, DefaultJsonOptions);
            }
            catch (Exception e)
            {
                CurrentAssertionChain
                    .WithDefaultIdentifier(Identifier)
                    .WithExpectation(
                        $"Expected HTTP response content to be deserializable to \"{typeof(TBody).Name}\", but deserialization threw an exception:",
                        assertionChain => assertionChain.FailWith(e.ToString())
                    );

                return new AndConstraint<HttpResponseMessageAssertions>(this);
            }

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

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Passes the raw <see cref="HttpResponseMessage"/> to <paramref name="assertion"/> for fully
    /// custom synchronous inspection of the response.
    /// </summary>
    /// <param name="assertion">
    /// A callback that receives the <see cref="HttpResponseMessage"/>. Use it to assert on any
    /// combination of status code, headers, and body that the built-in methods do not cover.
    /// </param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public AndConstraint<HttpResponseMessageAssertions> Satisfy(
        Action<HttpResponseMessage> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        string[] failuresFromInspector;

        using (AssertionScope assertionScope = new())
        {
            assertion(Subject);
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

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    private static void ThrowIfNull(object? value, string parameterName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value, parameterName);
#else
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
#endif
    }

    private static bool ContainsHeader(HttpResponseMessage response, string headerName)
    {
        return ContainsHeader(response.Headers, headerName)
            || (response.Content is not null && ContainsHeader(response.Content.Headers, headerName));
    }

    private static bool ContainsHeader(HttpHeaders headers, string headerName)
    {
        try
        {
            return headers.Contains(headerName);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static string[] GetHeaderValues(HttpResponseMessage response, string headerName)
    {
        string[] responseHeaderValues = GetHeaderValues(response.Headers, headerName);
        string[] contentHeaderValues = response.Content is not null
            ? GetHeaderValues(response.Content.Headers, headerName)
            : [];
        return [.. responseHeaderValues, .. contentHeaderValues];
    }

    private static string[] GetHeaderValues(HttpHeaders headers, string headerName)
    {
        try
        {
            return headers.TryGetValues(headerName, out IEnumerable<string>? values) ? [.. values] : [];
        }
        catch (InvalidOperationException)
        {
            return [];
        }
    }
}
