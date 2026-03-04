// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.

namespace Fluent.Client.AwesomeAssertions;

public sealed record ReceivedHttpResponse
{
    public required System.Net.HttpStatusCode StatusCode { get; init; }

    public required string Content { get; init; }

    public required KeyValuePair<string, string[]>[] Headers { get; init; }

    public bool IsEmpty() => string.IsNullOrWhiteSpace(Content);

    public bool IsSuccessStatusCode() => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);
}
