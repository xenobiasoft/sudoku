namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents the result of an API operation, encapsulating both success and failure states.
/// </summary>
/// <remarks>This class provides a standardized way to represent the outcome of an API call, including both
/// successful results and error information. Use the <see cref="Success(T)"/> method to create a successful result, or
/// the <see cref="Failure(string)"/> method to create a failure result.</remarks>
/// <typeparam name="T">The type of the value returned by the API operation when it succeeds.</typeparam>
public class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }

    public static ApiResult<T> Success(T value)
    {
        return new ApiResult<T> { IsSuccess = true, Value = value };
    }

    public static ApiResult<T> Failure(string error)
    {
        return new ApiResult<T> { IsSuccess = false, Error = error };
    }
}

/// <summary>
/// Represents the result of an API operation, indicating success or failure.
/// </summary>
/// <remarks>Use the <see cref="Success"/> method to create a successful result, or the <see
/// cref="Failure(string)"/>  method to create a failure result with an associated error message.</remarks>
public class ApiResult
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    public static ApiResult Success()
    {
        return new ApiResult { IsSuccess = true };
    }

    public static ApiResult Failure(string error)
    {
        return new ApiResult { IsSuccess = false, Error = error };
    }
}