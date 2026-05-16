namespace Sudoku.Blazor.Models;

public class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static ApiResult<T> Success(T value) =>
        new ApiResult<T> { IsSuccess = true, Value = value, StatusCode = 200 };

    public static ApiResult<T> Failure(string error, int statusCode = 0) =>
        new ApiResult<T> { IsSuccess = false, Error = error, StatusCode = statusCode };
}
