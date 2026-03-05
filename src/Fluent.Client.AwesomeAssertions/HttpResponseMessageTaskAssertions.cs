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
/// Provides fluent assertion methods for a <see cref="Task{HttpResponseMessage}"/> returned by
/// Fluent.Client HTTP methods such as <c>Get</c>, <c>Post</c>, <c>Put</c>, and <c>Delete</c>.
/// </summary>
/// <remarks>
/// Obtain an instance by calling <c>.Should()</c> on any Fluent.Client method result. The task is
/// awaited internally, so you do not need to await it before asserting.
/// <para>
/// For assertions on an already-materialised <see cref="HttpResponseMessage"/>, use
/// <see cref="HttpResponseMessageAssertions"/> via the <c>HttpResponseMessageExtensions.Should()</c>
/// extension instead.
/// </para>
/// </remarks>
public class HttpResponseMessageTaskAssertions(
    Task<HttpResponseMessage> instance,
    AssertionChain assertionChain
)
    : ReferenceTypeAssertions<Task<HttpResponseMessage>, HttpResponseMessageTaskAssertions>(
        instance,
        assertionChain
    )
{
    private readonly AssertionChain chain = assertionChain;

    protected override string Identifier => "http-response-task";

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    /// <summary>
    /// Shared <see cref="JsonSerializerOptions"/> used when deserializing response bodies in
    /// <c>Satisfy&lt;TBody&gt;</c> and <c>SucceedWith&lt;TBody&gt;</c>.
    /// </summary>
    /// <remarks>
    /// Default settings: property names are matched case-insensitively, trailing commas are
    /// allowed, and enums are serialized as strings. Replace this field globally to customise
    /// deserialization for your entire test suite, for example to register custom converters.
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
    /// Asserts that the HTTP response is both successful (status code 2xx) and has exactly
    /// <paramref name="expectedStatusCode"/>.
    /// </summary>
    /// <param name="expectedStatusCode">The exact status code the response must have.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <remarks>
    /// Unlike <c>HaveStatusCode</c>, this method enforces that the response is in the success
    /// range first, making the intent explicit when you want to confirm e.g. a <c>201 Created</c>
    /// and not just any arbitrary code.
    /// </remarks>
    [CustomAssertion]
    public async Task SucceedWith(
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
    [CustomAssertion]
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
    /// Asserts that the HTTP response has exactly <paramref name="expectedStatusCode"/>, without
    /// any constraint on whether the status is a success or failure code.
    /// </summary>
    /// <param name="expectedStatusCode">The exact status code the response must have.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <remarks>
    /// Use this method when asserting error codes such as 400, 401, or 422 where
    /// <c>SucceedWith</c> would fail the success check before comparing the code.
    /// </remarks>
    [CustomAssertion]
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

    /// <summary>
    /// Asserts that the HTTP response contains the expected header.
    /// </summary>
    [CustomAssertion]
    public async Task HaveHeader(
        string headerName,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(headerName, nameof(headerName));

        using HttpResponseMessage response = await Subject;

        bool headerExists = ContainsHeader(response, headerName);

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(headerExists)
            .FailWith($"Expected HTTP response to contain header \"{headerName}\", but it was not found.");
    }

    /// <summary>
    /// Asserts that the HTTP response contains the expected header with the expected value.
    /// </summary>
    [CustomAssertion]
    public async Task HaveHeaderWithValue(
        string headerName,
        string expectedValue,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(headerName, nameof(headerName));
        ThrowIfNull(expectedValue, nameof(expectedValue));

        using HttpResponseMessage response = await Subject;

        bool headerExists = ContainsHeader(response, headerName);

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(headerExists)
            .FailWith($"Expected HTTP response to contain header \"{headerName}\", but it was not found.");

        if (!headerExists)
        {
            return;
        }

        string[] actualHeaderValues = GetHeaderValues(response, headerName);
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
    }

    /// <summary>
    /// Asserts that the HTTP response contains all expected headers.
    /// </summary>
    [CustomAssertion]
    public async Task HaveHeaders(params string[] headerNames)
    {
        ThrowIfNull(headerNames, nameof(headerNames));

        using HttpResponseMessage response = await Subject;

        foreach (string headerName in headerNames)
        {
            ThrowIfNull(headerName, nameof(headerName));
        }

        string[] missingHeaders = [.. headerNames.Where(name => !ContainsHeader(response, name))];
        string expectedList = string.Join(", ", headerNames.Select(name => $"\"{name}\""));
        string missingList = string.Join(", ", missingHeaders.Select(name => $"\"{name}\""));

        chain
            .ForCondition(missingHeaders.Length == 0)
            .FailWith(
                $"Expected HTTP response to contain headers {expectedList}, but the following were not found: {missingList}."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a failure with the specific status code.
    /// </summary>
    [CustomAssertion]
    public async Task FailWith(
        System.Net.HttpStatusCode expectedStatusCode,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!response.IsSuccessStatusCode && response.StatusCode == expectedStatusCode)
            .FailWith(
                $"Expected HTTP response to fail with status code {(int)expectedStatusCode} ({expectedStatusCode}), "
                    + $"but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response has a 404 Not Found status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeNotFound(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.NotFound, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 401 Unauthorized status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeUnauthorized(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.Unauthorized, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 400 Bad Request status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeBadRequest(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.BadRequest, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 201 Created status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeCreated(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.Created, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 204 No Content status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeNoContent(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.NoContent, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 403 Forbidden status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeForbidden(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.Forbidden, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has a 409 Conflict status code.
    /// </summary>
    [CustomAssertion]
    public async Task BeConflict(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    ) => await HaveStatusCode(System.Net.HttpStatusCode.Conflict, because, becauseArgs);

    /// <summary>
    /// Asserts that the HTTP response has the expected Content-Type header.
    /// </summary>
    [CustomAssertion]
    public async Task HaveContentType(
        string expectedMediaType,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(expectedMediaType, nameof(expectedMediaType));

        using HttpResponseMessage response = await Subject;

        string? actualContentType = response.Content?.Headers?.ContentType?.MediaType;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(
                string.Equals(actualContentType, expectedMediaType, StringComparison.OrdinalIgnoreCase)
            )
            .FailWith(
                $"Expected HTTP response to have Content-Type \"{expectedMediaType}\", but found \"{actualContentType ?? "(none)"}\"."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response indicates a redirect (3xx status code).
    /// </summary>
    [CustomAssertion]
    public async Task BeRedirect(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        int statusCode = (int)response.StatusCode;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(statusCode >= 300 && statusCode <= 399)
            .FailWith(
                $"Expected HTTP response to be a redirect (3xx), but found {statusCode} ({response.ReasonPhrase})."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response is a redirect (3xx) whose <c>Location</c> header equals
    /// <paramref name="expectedUrl"/>.
    /// </summary>
    /// <param name="expectedUrl">The URL the <c>Location</c> header must match.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <remarks>
    /// The <c>Location</c> header comparison is case-insensitive. Use this assertion to verify
    /// login redirects, permanent moves (301), or any endpoint that returns a 3xx with an
    /// explicit target URL.
    /// </remarks>
    [CustomAssertion]
    public async Task BeRedirectedTo(
        string expectedUrl,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        ThrowIfNull(expectedUrl, nameof(expectedUrl));

        using HttpResponseMessage response = await Subject;

        int statusCode = (int)response.StatusCode;

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(statusCode >= 300 && statusCode <= 399)
            .FailWith(
                $"Expected HTTP response to be a redirect (3xx), but found {statusCode} ({response.ReasonPhrase})."
            );

        if (statusCode < 300 || statusCode > 399)
        {
            return;
        }

        string? actualLocation = response.Headers.Location?.ToString();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(string.Equals(actualLocation, expectedUrl, StringComparison.OrdinalIgnoreCase))
            .FailWith(
                $"Expected HTTP response to redirect to \"{expectedUrl}\", but found \"{actualLocation ?? "(none)"}\"."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response has an empty body.
    /// </summary>
    [CustomAssertion]
    public async Task HaveEmptyBody(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        string content = await response.Content.ReadAsStringAsync();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(string.IsNullOrWhiteSpace(content))
            .FailWith(
                $"Expected HTTP response to have an empty body, but found content with length {content.Length}."
            );
    }

    /// <summary>
    /// Asserts that the HTTP response has a non-empty body.
    /// </summary>
    [CustomAssertion]
    public async Task HaveNonEmptyBody(
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        using HttpResponseMessage response = await Subject;

        string content = await response.Content.ReadAsStringAsync();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(content))
            .FailWith("Expected HTTP response to have a non-empty body, but the body was empty.");
    }

    /// <summary>
    /// Asserts that the HTTP response is successful (2xx) and that the response body, deserialized
    /// to <typeparamref name="TBody"/>, satisfies the provided <paramref name="assertion"/>.
    /// </summary>
    /// <typeparam name="TBody">The type to deserialize the JSON response body into.</typeparam>
    /// <param name="assertion">A callback that receives the deserialized body and performs further assertions.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <remarks>
    /// Deserialization uses <see cref="DefaultJsonOptions"/>. The assertion fails immediately if
    /// the status code is not 2xx — use <c>Satisfy&lt;TBody&gt;</c> instead when you need to
    /// assert on error responses that carry a JSON body (e.g. <c>ProblemDetails</c> on 400).
    /// </remarks>
    [CustomAssertion]
    public async Task SucceedWith<TBody>(
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

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(response.IsSuccessStatusCode)
            .FailWith(
                $"Expected HTTP response to be successful, but found {(int)response.StatusCode} ({response.ReasonPhrase})."
            );

        if (!CurrentAssertionChain.Succeeded)
        {
            return;
        }

        string content = await response.Content.ReadAsStringAsync();

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrWhiteSpace(content))
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (!CurrentAssertionChain.Succeeded)
        {
            return;
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

                return;
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
    }

    /// <summary>
    /// Deserializes the response body to <typeparamref name="TBody"/> regardless of the status
    /// code, then runs <paramref name="assertion"/> on the result.
    /// </summary>
    /// <typeparam name="TBody">The type to deserialize the JSON response body into.</typeparam>
    /// <param name="assertion">A callback that receives the deserialized body and performs further assertions.</param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <remarks>
    /// Unlike <c>SucceedWith&lt;TBody&gt;</c>, this overload does <b>not</b> require a 2xx status
    /// code. Use it when testing endpoints that return a structured JSON body on error, such as
    /// <c>ProblemDetails</c> on a 400 or 422 response.
    /// </remarks>
    /// <example>
    /// <code>
    /// await client.Post("/users", invalidData)
    ///     .Should()
    ///     .Satisfy&lt;ProblemDetails&gt;(problem => problem.Status.Should().Be(400));
    /// </code>
    /// </example>
    [CustomAssertion]
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

        ReceivedHttpResponse receivedResponse = new()
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync(),
            Headers =
            [
                .. response.Headers.Select(x => new KeyValuePair<string, string[]>(x.Key, [.. x.Value])),
            ],
        };

        // TODO: When 400 can deserialize problem response
        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!receivedResponse.IsEmpty())
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (CurrentAssertionChain.Succeeded)
        {
            string[] failuresFromInspector;

            using (AssertionScope assertionScope = new())
            {
                TBody? body;

                try
                {
                    body = JsonSerializer.Deserialize<TBody>(receivedResponse.Content, DefaultJsonOptions);
                }
                catch (Exception e)
                {
                    CurrentAssertionChain
                        .WithDefaultIdentifier(Identifier)
                        .WithExpectation(
                            $"Expected HTTP response content to be deserializable to \"{typeof(TBody).Name}\", but deserialization threw an exception:",
                            assertionChain => assertionChain.FailWith(e.ToString())
                        );

                    return;
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
        }
    }

    /// <summary>
    /// Asserts that the deserialized HTTP response body satisfies the provided asynchronous assertion.
    /// </summary>
    [CustomAssertion]
    public async Task Satisfy<TBody>(
        Func<TBody, Task> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        using HttpResponseMessage response = await Subject;

        ReceivedHttpResponse receivedResponse = new()
        {
            StatusCode = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync(),
            Headers =
            [
                .. response.Headers.Select(x => new KeyValuePair<string, string[]>(x.Key, [.. x.Value])),
            ],
        };

        chain
            .BecauseOf(because, becauseArgs)
            .ForCondition(!receivedResponse.IsEmpty())
            .FailWith(
                $"Expected HTTP response body to be deserializable to {typeof(TBody)}, but it was null."
            );

        if (CurrentAssertionChain.Succeeded)
        {
            string[] failuresFromInspector;

            using (AssertionScope assertionScope = new())
            {
                TBody? body;

                try
                {
                    body = JsonSerializer.Deserialize<TBody>(receivedResponse.Content, DefaultJsonOptions);
                }
                catch (Exception e)
                {
                    CurrentAssertionChain
                        .WithDefaultIdentifier(Identifier)
                        .WithExpectation(
                            $"Expected HTTP response content to be deserializable to \"{typeof(TBody).Name}\", but deserialization threw an exception:",
                            assertionChain => assertionChain.FailWith(e.ToString())
                        );

                    return;
                }

                await assertion(body!);
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

    /// <summary>
    /// Passes the raw <see cref="HttpResponseMessage"/> to <paramref name="assertion"/> for fully
    /// custom, synchronous inspection of the response.
    /// </summary>
    /// <param name="assertion">
    /// A callback that receives the awaited <see cref="HttpResponseMessage"/>. Use it to assert on
    /// any combination of status code, headers, and body that the built-in methods do not cover.
    /// </param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    /// <example>
    /// <code>
    /// await client.Get("/data").Should().Satisfy(response =>
    /// {
    ///     response.StatusCode.Should().Be(HttpStatusCode.OK);
    ///     response.Headers.ETag.Should().NotBeNull();
    /// });
    /// </code>
    /// </example>
    [CustomAssertion]
    public async Task Satisfy(
        Action<HttpResponseMessage> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        using HttpResponseMessage response = await Subject;

        string[] failuresFromInspector;

        using (AssertionScope assertionScope = new())
        {
            assertion(response);
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

    /// <summary>
    /// Passes the raw <see cref="HttpResponseMessage"/> to an asynchronous
    /// <paramref name="assertion"/> for fully custom inspection of the response.
    /// </summary>
    /// <param name="assertion">
    /// An async callback that receives the awaited <see cref="HttpResponseMessage"/>. Use this
    /// overload when your assertion itself needs to <c>await</c>, for example to read the body
    /// stream or call async helper methods.
    /// </param>
    /// <param name="because">A reason to include in the failure message.</param>
    /// <param name="becauseArgs">Format arguments for <paramref name="because"/>.</param>
    [CustomAssertion]
    public async Task Satisfy(
        Func<HttpResponseMessage, Task> assertion,
        [StringSyntax("CompositeFormat")] string because = "",
        params object[] becauseArgs
    )
    {
        if (assertion is null)
        {
            throw new ArgumentNullException(nameof(assertion));
        }

        using HttpResponseMessage response = await Subject;

        string[] failuresFromInspector;

        using (AssertionScope assertionScope = new())
        {
            await assertion(response);
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
