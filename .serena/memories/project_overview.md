# Fluent Framework - Project Overview

## Purpose
Fluent Framework is a modern collection of independent libraries for .NET applications with fluent, expressive APIs.
Created by Leszek Pomianowski (lepo.co). Licensed under MIT.

## Current Packages
1. **Fluent.Client** (`src/Fluent.Client/`) - Fluent HTTP client wrapper for clean, chainable requests
2. **Fluent.Client.AwesomeAssertions** (`src/Fluent.Client.AwesomeAssertions/`) - Expressive HTTP response assertions for integration tests

## Tech Stack
- **.NET SDK**: 10.x (primary), also targets net8.0, net472, net481
- **C# Language Version**: 14.0
- **Testing**: xunit.v3 (3.2.1) + AwesomeAssertions (9.3.0)
- **Package Management**: Central Package Management (Directory.Packages.props)
- **Formatter**: CSharpier (printWidth: 110)
- **Solution file**: `Fluent.slnx` (modern XML solution format)
- **NuGet**: packages published to nuget.org
- **CI**: GitHub Actions (Windows runner)

## Solution Structure
```
Fluent.slnx                           # Solution file
Directory.Build.props                 # Shared build properties (version, lang, etc.)
Directory.Build.targets               # Shared build targets
Directory.Packages.props              # Central Package Management
src/
  Fluent.Client/                      # HTTP client library
  Fluent.Client.AwesomeAssertions/    # Test assertions library
tests/
  Fluent.Client.UnitTests/            # Unit tests for Fluent.Client
  Fluent.Client.AwesomeAssertions.UnitTests/  # Unit tests for assertions
build/                                # Build assets (icons, etc.)
docs/                                 # Documentation
.github/                             # GitHub config, workflows
```

## Key Build Properties
- `Nullable`: enabled
- `ImplicitUsings`: enabled
- `AllowUnsafeBlocks`: true
- `EnforceCodeStyleInBuild`: true
- Test projects detected via `MSBuildProjectName.Contains('Test')`
- Core projects are packable, test projects are not
