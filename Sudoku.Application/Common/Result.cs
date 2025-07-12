namespace Sudoku.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, T? value, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error, new List<string> { error });
    public static Result<T> Failure(List<string> errors) => new(false, default, errors.FirstOrDefault(), errors);

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess ? Result<TNew>.Success(mapper(Value!)) : Result<TNew>.Failure(Errors);
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error, new List<string> { error });
    public static Result Failure(List<string> errors) => new(false, errors.FirstOrDefault(), errors);
}