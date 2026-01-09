// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

namespace Fluent.Client;

/// <summary>
/// Specifies the type of authorization to use for an HTTP request.
/// </summary>
public enum AuthorizationType
{
    /// <summary>
    /// Bearer authentication (often used with OAuth 2.0 and JWT).
    /// </summary>
    Bearer,

    /// <summary>
    /// Basic authentication (username and password encoded in Base64).
    /// </summary>
    Basic,

    /// <summary>
    /// Digest authentication.
    /// </summary>
    Digest,

    /// <summary>
    /// API Key authentication (often passed in a custom header or query parameter).
    /// </summary>
    ApiKey,

    /// <summary>
    /// OAuth authentication.
    /// </summary>
    OAuth,
}
