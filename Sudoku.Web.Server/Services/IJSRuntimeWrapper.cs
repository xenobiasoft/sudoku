namespace Sudoku.Web.Server.Services;

public interface IJsRuntimeWrapper
{
    ValueTask<string> GetAsync(string key);
    ValueTask SetAsync(string key, string value);
}