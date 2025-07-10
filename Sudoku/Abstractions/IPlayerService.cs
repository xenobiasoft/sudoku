namespace XenobiaSoft.Sudoku.Abstractions;

public interface IPlayerService
{
    Task<string> CreateNewAsync();
}