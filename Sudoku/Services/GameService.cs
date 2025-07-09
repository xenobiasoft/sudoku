using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public class GameService : IGameService
{
    public Task<IEnumerable<GameStateMemory>> GetGamesForPlayerAsync(string alias)
    {
        throw new NotImplementedException();
    }
}