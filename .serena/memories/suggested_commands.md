# Fluent Framework - Suggested Commands

## Build
```powershell
dotnet build Fluent.slnx --configuration Release
dotnet build Fluent.slnx --configuration Debug
```

## Restore
```powershell
dotnet restore Fluent.slnx
```

## Test
```powershell
# Run all tests
dotnet test Fluent.slnx

# Run specific test project
dotnet test tests\Fluent.Client.UnitTests\Fluent.Client.UnitTests.csproj
dotnet test tests\Fluent.Client.AwesomeAssertions.UnitTests\Fluent.Client.AwesomeAssertions.UnitTests.csproj
```

## Format (CSharpier)
```powershell
dotnet csharpier .
dotnet csharpier --check .
```

## Pack (NuGet)
```powershell
dotnet pack Fluent.slnx --configuration Release
```

## Git (Windows)
```powershell
git --no-pager status
git --no-pager diff
git --no-pager log --oneline -20
```

## System (Windows/PowerShell)
- List files: `Get-ChildItem` (or `ls`)
- Search text: `Select-String -Pattern "text" -Path "*.cs" -Recurse`
- Find files: `Get-ChildItem -Recurse -Filter "*.cs"`
