# Error Handling Patterns

## Domain Exceptions
```csharp
public class InvalidMoveException : DomainException
{
    public InvalidMoveException(int row, int column, int value)
        : base($"Invalid move: {value} at position ({row}, {column})")
    {
    }
}
```

## Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

## Rules
- Use `Result<T>` for expected failures — never throw exceptions for them
- Use domain exceptions only for invariant violations inside aggregates
- Validate at system boundaries (user input, external APIs); trust internal code
- Don't add error handling for scenarios that can't happen
