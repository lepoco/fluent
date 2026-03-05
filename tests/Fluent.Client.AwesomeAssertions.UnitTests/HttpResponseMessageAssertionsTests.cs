// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit.Sdk;

namespace Fluent.Client.AwesomeAssertions.UnitTests;

// ReSharper disable AccessToDisposedClosure
public sealed class HttpResponseMessageAssertionsTests
{
    [Fact]
    public void Succeed_ShouldPass_WhenStatusCodeIsOk()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Should().Succeed("because the status is 200 OK");
    }

    [Fact]
    public void Succeed_ShouldFail_WhenStatusCodeIsNotSuccess()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest);
        Action act = () => response.Should().Succeed();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SucceedWith_ShouldPass_WhenStatusCodeMatches()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Created);
        response.Should().SucceedWith(HttpStatusCode.Created);
    }

    [Fact]
    public void SucceedWith_ShouldFail_WhenStatusCodeDoesNotMatch()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().SucceedWith(HttpStatusCode.Created);
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void Fail_ShouldPass_WhenStatusCodeIsError()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest);
        response.Should().Fail();
    }

    [Fact]
    public void Fail_ShouldFail_WhenStatusCodeIsSuccess()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().Fail();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveStatusCode_ShouldPass_WhenStatusCodeMatches()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Forbidden);
        response.Should().HaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public void HaveStatusCode_ShouldFail_WhenStatusCodeDoesNotMatch()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().HaveStatusCode(HttpStatusCode.Forbidden);
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveHeader_ShouldPass_WhenHeaderExists()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Headers.Add("X-Custom-Header", "value");
        response.Should().HaveHeader("X-Custom-Header");
    }

    [Fact]
    public void HaveHeader_ShouldFail_WhenHeaderDoesNotExist()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().HaveHeader("X-Missing-Header");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveHeader_WithValue_ShouldPass_WhenHeaderHasExpectedValue()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Headers.Add("X-Custom", "expected-value");
        response.Should().HaveHeader("X-Custom", "expected-value");
    }

    [Fact]
    public void HaveHeaderWithValue_WithValue_ShouldFail_WhenHeaderHasWrongValue()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Headers.Add("X-Custom", "actual-value");

        Func<AndConstraint<HttpResponseMessageAssertions>> action = () =>
            response.Should().HaveHeaderWithValue("X-Custom", "wrong-value");

        action.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveHeaderWithValue_WithValue_ShouldFail_WhenHeaderDoesNotExist()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().HaveHeaderWithValue("X-Missing", "some-value");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveHeader_ShouldPass_WhenContentHeaderExists()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("content"),
        };
        response.Should().HaveHeader("Content-Type");
    }

    [Fact]
    public void HaveHeaders_ShouldPass_WhenAllHeadersExist()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Headers.Add("X-First-Header", "first");
        response.Headers.Add("X-Second-Header", "second");
        response.Should().HaveHeaders("X-First-Header", "X-Second-Header");
    }

    [Fact]
    public void HaveHeaders_ShouldFail_WhenSomeHeadersAreMissing()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        response.Headers.Add("X-First-Header", "first");
        Action act = () =>
            response.Should().HaveHeaders("X-First-Header", "X-Second-Header", "X-Third-Header");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void FailWith_ShouldPass_WhenStatusCodeMatches()
    {
        using HttpResponseMessage response = new(HttpStatusCode.NotFound);
        response.Should().FailWith(HttpStatusCode.NotFound);
    }

    [Fact]
    public void FailWith_ShouldFail_WhenStatusCodeDoesNotMatch()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest);
        Action act = () => response.Should().FailWith(HttpStatusCode.NotFound);
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeNotFound_ShouldPass_WhenStatusCodeIs404()
    {
        using HttpResponseMessage response = new(HttpStatusCode.NotFound);
        response.Should().BeNotFound();
    }

    [Fact]
    public void BeNotFound_ShouldFail_WhenStatusCodeIsNot404()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeNotFound();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeUnauthorized_ShouldPass_WhenStatusCodeIs401()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Unauthorized);
        response.Should().BeUnauthorized();
    }

    [Fact]
    public void BeUnauthorized_ShouldFail_WhenStatusCodeIsNot401()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeUnauthorized();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeBadRequest_ShouldPass_WhenStatusCodeIs400()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest);
        response.Should().BeBadRequest();
    }

    [Fact]
    public void BeBadRequest_ShouldFail_WhenStatusCodeIsNot400()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeBadRequest();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeCreated_ShouldPass_WhenStatusCodeIs201()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Created);
        response.Should().BeCreated();
    }

    [Fact]
    public void BeCreated_ShouldFail_WhenStatusCodeIsNot201()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeCreated();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeNoContent_ShouldPass_WhenStatusCodeIs204()
    {
        using HttpResponseMessage response = new(HttpStatusCode.NoContent);
        response.Should().BeNoContent();
    }

    [Fact]
    public void BeNoContent_ShouldFail_WhenStatusCodeIsNot204()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeNoContent();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeForbidden_ShouldPass_WhenStatusCodeIs403()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Forbidden);
        response.Should().BeForbidden();
    }

    [Fact]
    public void BeForbidden_ShouldFail_WhenStatusCodeIsNot403()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeForbidden();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeConflict_ShouldPass_WhenStatusCodeIs409()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Conflict);
        response.Should().BeConflict();
    }

    [Fact]
    public void BeConflict_ShouldFail_WhenStatusCodeIsNot409()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeConflict();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveContentType_ShouldPass_WhenContentTypeMatches()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", new MediaTypeHeaderValue("application/json")),
        };
        response.Should().HaveContentType("application/json");
    }

    [Fact]
    public void HaveContentType_ShouldFail_WhenContentTypeDoesNotMatch()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", new MediaTypeHeaderValue("application/json")),
        };
        Action act = () => response.Should().HaveContentType("text/xml");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeRedirect_ShouldPass_WhenStatusCodeIs3xx()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Redirect)
        {
            Headers = { Location = new Uri("https://example.com/new") },
        };
        response.Should().BeRedirect();
    }

    [Fact]
    public void BeRedirect_ShouldFail_WhenStatusCodeIsNot3xx()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeRedirect();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeRedirectedTo_ShouldPass_WhenLocationMatches()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Redirect)
        {
            Headers = { Location = new Uri("https://example.com/new-location") },
        };
        response.Should().BeRedirectedTo("https://example.com/new-location");
    }

    [Fact]
    public void BeRedirectedTo_ShouldFail_WhenLocationDoesNotMatch()
    {
        using HttpResponseMessage response = new(HttpStatusCode.Redirect)
        {
            Headers = { Location = new Uri("https://example.com/actual") },
        };
        Action act = () => response.Should().BeRedirectedTo("https://example.com/expected");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeRedirectedTo_ShouldFail_WhenStatusCodeIsNot3xx()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);
        Action act = () => response.Should().BeRedirectedTo("https://example.com/new");
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveEmptyBody_ShouldPass_WhenBodyIsEmpty()
    {
        using HttpResponseMessage response = new(HttpStatusCode.NoContent)
        {
            Content = new StringContent(""),
        };
        response.Should().HaveEmptyBody();
    }

    [Fact]
    public void HaveEmptyBody_ShouldFail_WhenBodyHasContent()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\": 1}"),
        };
        Action act = () => response.Should().HaveEmptyBody();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void HaveNonEmptyBody_ShouldPass_WhenBodyHasContent()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\": 1}"),
        };
        response.Should().HaveNonEmptyBody();
    }

    [Fact]
    public void HaveNonEmptyBody_ShouldFail_WhenBodyIsEmpty()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK) { Content = new StringContent("") };
        Action act = () => response.Should().HaveNonEmptyBody();
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SucceedWithBody_ShouldPass_WhenSuccessAndBodyMatchesAssertion()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new TestBody { Id = 99, Name = "Success" }),
                new MediaTypeHeaderValue("application/json")
            ),
        };

        response
            .Should()
            .SucceedWith<TestBody>(body =>
            {
                body.Id.Should().Be(99);
                body.Name.Should().Be("Success");
            });
    }

    [Fact]
    public void SucceedWithBody_ShouldFail_WhenResponseIsNotSuccess()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{}"),
        };

        Action act = () =>
            response
                .Should()
                .SucceedWith<TestBody>(body =>
                {
                    body.Id.Should().Be(1);
                });
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SucceedWithBody_ShouldFail_WhenBodyAssertionFails()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new TestBody { Id = 1, Name = "Wrong" }),
                new MediaTypeHeaderValue("application/json")
            ),
        };

        Action act = () =>
            response
                .Should()
                .SucceedWith<TestBody>(body =>
                {
                    body.Id.Should().Be(999);
                });
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SatisfyBody_ShouldPass_WhenBodyMatchesAssertion()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new TestBody { Id = 42, Name = "The Answer" }),
                new MediaTypeHeaderValue("application/json")
            ),
        };

        response
            .Should()
            .Satisfy<TestBody>(body =>
            {
                body.Id.Should().Be(42);
                body.Name.Should().Be("The Answer");
            });
    }

    [Fact]
    public void SatisfyBody_ShouldFail_WhenBodyAssertionFails()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new TestBody { Id = 1, Name = "Actual" }),
                new MediaTypeHeaderValue("application/json")
            ),
        };

        Action act = () =>
            response
                .Should()
                .Satisfy<TestBody>(body =>
                {
                    body.Id.Should().Be(999);
                });
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SatisfyBody_ShouldFail_WhenBodyIsInvalidJson()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{invalid json}", new MediaTypeHeaderValue("application/json")),
        };

        Action act = () =>
            response
                .Should()
                .Satisfy<TestBody>(body =>
                {
                    body.Id.Should().Be(1);
                });
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void SatisfyResponse_ShouldPass_WhenInspectorSucceeds()
    {
        using HttpResponseMessage response = new(HttpStatusCode.OK);

        response
            .Should()
            .Satisfy(r =>
            {
                r.StatusCode.Should().Be(HttpStatusCode.OK);
            });
    }

    [Fact]
    public void SatisfyResponse_ShouldFail_WhenInspectorFails()
    {
        using HttpResponseMessage response = new(HttpStatusCode.BadRequest);

        Action act = () =>
            response
                .Should()
                .Satisfy(r =>
                {
                    r.StatusCode.Should().Be(HttpStatusCode.OK);
                });
        act.Should().Throw<XunitException>();
    }

    private sealed record TestBody
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}
