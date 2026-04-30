namespace Sudoku.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, T? value, string? error, string? errorCode = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error, null, [error]);
    public static Result<T> Failure(string error, string errorCode) => new(false, default, error, errorCode, [error]);
    public static Result<T> Failure(List<string> errors) => new(false, default, errors.FirstOrDefault(), null, errors);

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess ? Result<TNew>.Success(mapper(Value!)) : Result<TNew>.Failure(Errors);
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, string? error, string? errorCode = null, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error, null, [error]);
    public static Result Failure(string error, string errorCode) => new(false, error, errorCode, [error]);
    public static Result Failure(List<string> errors) => new(false, errors.FirstOrDefault(), null, errors);
}