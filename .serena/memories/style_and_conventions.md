# Fluent Framework - Code Style & Conventions

## General
- **Indentation**: 4 spaces for C#, 2 spaces for XML/JSON/YAML/props/csproj
- **Line endings**: CRLF (Windows)
- **Encoding**: UTF-8
- **No final newline** (except .md files)
- **CSharpier** formatter with printWidth: 110

## C# Style
- **File-scoped namespaces**: required (warning)
- **Usings**: outside namespace (warning)
- **Nullable**: enabled globally
- **Implicit usings**: enabled
- **var usage**: discouraged (`csharp_style_var_elsewhere = false:warning`)
- **Braces**: prefer always (`csharp_prefer_braces = true:suggestion`)
- **Expression-bodied members**: disabled for most (except lambdas)
- **Pattern matching**: preferred
- **Static local functions**: preferred (warning)

## Naming Conventions
- **PascalCase**: classes, structs, enums, delegates, public members, constants, public/internal fields, static readonly fields
- **camelCase**: parameters, private readonly fields (with `_` prefix for private/protected fields)
- **Interfaces**: PascalCase with `I` prefix
- **Async methods**: should end with `Async` suffix (silent severity)

## File Header (required for all .cs files)
```
// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Fluent Framework Contributors.
// All Rights Reserved.
```

## Testing
- **Framework**: xunit.v3
- **Assertions**: AwesomeAssertions
- **Test project naming**: `{ProjectName}.UnitTests`
- **Test file naming**: `{ClassName}Tests.cs`
- **Test projects use Stubs directory** for test doubles
- **Test projects target**: net10.0 only

## Code Analysis
- Enforced in build (`EnforceCodeStyleInBuild = true`)
- `CA1707` (underscores in names) suppressed (for test method names)
- Various CA rules configured as warnings
