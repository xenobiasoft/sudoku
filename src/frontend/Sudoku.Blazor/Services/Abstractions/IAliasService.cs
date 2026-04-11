namespace Sudoku.Blazor.Services.Abstractions;

public interface IAliasService
{
    Task<string> GetAliasAsync();
}