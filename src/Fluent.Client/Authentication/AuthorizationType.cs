// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

namespace Fluent.Client.Authentication;

/// <summary>
/// Specifies the authentication scheme used in the <c>Authorization</c> request header.
/// The string representation of each member is used directly as the scheme token.
/// </summary>
public enum AuthorizationType
{
    /// <summary>
    /// Bearer token, typically used with OAuth 2.0 and JWT.
    /// Produces: <c>Authorization: Bearer &lt;token&gt;</c>
    /// </summary>
    Bearer,

    /// <summary>
    /// HTTP Basic authentication. Credentials are Base64-encoded as <c>username:password</c>.
    /// Produces: <c>Authorization: Basic &lt;base64&gt;</c>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7617">RFC 7617</see>.
    /// </summary>
    Basic,

    /// <summary>
    /// HTTP Digest authentication (token68 form only).
    /// Full challenge/response Digest auth (realm, nonce, qop) is not supported here — use <c>WithHeader</c> directly for that.
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7616">RFC 7616</see>.
    /// </summary>
    Digest,

    /// <summary>
    /// API key authentication. The key is placed in a custom header (default: <c>api-key</c>) without a scheme prefix.
    /// </summary>
    ApiKey,

    /// <summary>
    /// OAuth token authentication.
    /// Produces: <c>Authorization: OAuth &lt;token&gt;</c>
    /// </summary>
    OAuth,

    /// <summary>
    /// HTTP Origin-Bound Authentication.
    /// Produces: <c>Authorization: HOBA &lt;token&gt;</c>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7486">RFC 7486</see>.
    /// </summary>
    HOBA,

    /// <summary>
    /// Mutual authentication.
    /// Produces: <c>Authorization: Mutual &lt;token&gt;</c>
    /// </summary>
    Mutual,

    /// <summary>
    /// Negotiate authentication (Kerberos / SPNEGO).
    /// Produces: <c>Authorization: Negotiate &lt;token&gt;</c>
    /// </summary>
    Negotiate,

    /// <summary>
    /// Voluntary Application Server Identification, used for Web Push.
    /// Produces: <c>Authorization: VAPID &lt;token&gt;</c>
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc8292">RFC 8292</see>.
    /// </summary>
    VAPID,
}
