namespace Sudoku.Web.Server.Services.Abstractions;

public interface IAliasService
{
    Task<string> GetAliasAsync();
}