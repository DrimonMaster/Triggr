# TriggerProject

Typed event bus library for .NET 8.

## Project structure

```
TriggerProject/          # Class library (the actual library)
TriggerProject.Tests/    # xUnit tests
```

## Build & test

```
dotnet build
dotnet test
```

## Conventions

- Target framework: `net10.0`
- Nullable reference types: enabled
- Warnings as errors: enabled
- No `ImplicitUsings` workarounds — add explicit usings where needed in library code
- xUnit for all tests; no mocking frameworks unless added explicitly
- One test class per production class, named `<ClassName>Tests`
