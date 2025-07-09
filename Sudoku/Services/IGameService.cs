using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public interface IGameService
{
    Task<IEnumerable<GameStateMemory>> GetGamesForPlayerAsync(string alias);
}