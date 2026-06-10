// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

using System.Text;

namespace Fluent.Client.Authentication;

/// <summary>
/// Builds the Authorization header name and value from the provided credentials and scheme.
/// </summary>
public static class AuthorizationHeaderBuilder
{
    /// <summary>
    /// Returns the header name and value for the given credentials.
    /// When <paramref name="kind"/> is provided it takes precedence; otherwise the scheme is inferred
    /// from whichever credentials are supplied.
    /// </summary>
    public static (string Name, string Value) Build(
        string? username = null,
        string? password = null,
        string? token = null,
        string? key = null,
        string? headerName = null,
        AuthorizationType? kind = null
    ) =>
        kind switch
        {
            AuthorizationType.Basic => ForBasic(username, password, headerName),
            AuthorizationType.ApiKey => ForApiKey(key, headerName),
            not null => ForToken(token, headerName, kind.Value),
            null => InferFromCredentials(username, password, token, key, headerName),
        };

    private static (string, string) InferFromCredentials(
        string? username,
        string? password,
        string? token,
        string? key,
        string? headerName
    )
    {
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            return ForBasic(username, password, headerName);
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            return ForToken(token, headerName, AuthorizationType.Bearer);
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            return ForApiKey(key, headerName);
        }

        throw new ArgumentException("Provide a token, an API key, or a username and password.");
    }

    private static (string, string) ForBasic(string? username, string? password, string? headerName)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        string encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        return (headerName ?? "Authorization", $"Basic {encoded}");
    }

    private static (string, string) ForToken(string? token, string? headerName, AuthorizationType scheme)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentNullException(nameof(token));
        }

        return (headerName ?? "Authorization", $"{scheme} {token}");
    }

    private static (string, string) ForApiKey(string? key, string? headerName)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        return (headerName ?? "api-key", key);
    }
}
