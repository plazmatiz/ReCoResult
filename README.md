# ReCoResult

A lightweight, expressive implementation of the Result pattern for .NET.

## Features

- Non-generic `Result` and generic `Result<T>` representations.
- Strongly-typed errors with extensible error codes.
- Exception-safe execution via `Result.Try` and `Result<T>.Try`.
- Implicit boolean operators for concise validation checks.
- Fluent API for chaining errors and informational messages.
- Full support for nullable reference types (`<Nullable>enable</Nullable>`).

## Installation

Install via the .NET CLI:

```bash
dotnet add package ReCoResult
```

Or via the NuGet Package Manager:

```powershell
Install-Package ReCoResult
```

## Quick Start

### Basic Usage

```csharp
using ReCo.Result;

// Successful operations
Result successResult = Result.Success();
Result<int> valueResult = Result<int>.Success(42);

// Failed operations
Result failResult = Result.Fail("Resource not found.");
Result<int> failWithValue = Result<int>.Fail("Invalid identifier.");

// Checking results
if (valueResult) // Evaluates via implicit operator bool
{
    Console.WriteLine($"Computed value: {valueResult.Value}");
}
else
{
    foreach (var error in valueResult.Errors)
    {
        Console.WriteLine($"Error [{error.ErrorCode}]: {error.Message}");
    }
}
```

### Exception Handling with `Try`

Safely wrap delegates and map unhandled exceptions to domain errors:

```csharp
Result<string> result = Result<string>.Try(
    () => File.ReadAllText("config.json"),
    ex => new Error(ex.Message, ErrorCode.Unexpected)
);
```

### Fluent Chaining

```csharp
var result = Result.Success()
    .WithSuccess("Step 1 completed.")
    .WithError(new Error("Warning: low disk space.", ErrorCode.Unexpected));
```

## License

This project is licensed under the MIT License.