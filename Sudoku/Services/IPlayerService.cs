namespace XenobiaSoft.Sudoku.Services;

public interface IPlayerService
{
    Task<string> CreateNewAsync();
}