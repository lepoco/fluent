# Fluent Framework - Task Completion Checklist

When a coding task is completed, verify the following:

1. **Build**: `dotnet build Fluent.slnx --configuration Release` passes without errors
2. **Tests**: `dotnet test Fluent.slnx` passes without failures
3. **Format**: `dotnet csharpier --check .` passes (run `dotnet csharpier .` to fix)
4. **File headers**: All new .cs files include the MIT license header
5. **Naming**: Follows PascalCase/camelCase conventions per .editorconfig
6. **Nullable**: No nullable warnings in new code
7. **Documentation**: XML doc comments on public APIs (`GenerateDocumentationFile` is enabled for core projects)
