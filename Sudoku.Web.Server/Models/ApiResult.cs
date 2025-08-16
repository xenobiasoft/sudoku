namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents the result of an API call
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
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