namespace Sudoku.Blazor.Services.Abstractions;

public interface IJsRuntimeWrapper
{
    ValueTask<string> GetAsync(string key);
    ValueTask SetAsync(string key, string value);
}