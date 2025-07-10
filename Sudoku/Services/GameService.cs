using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public class GameService(IPersistentGameStateStorage storage) : IGameService
{
    public Task DeleteGameAsync(string alias, string gameId)
    {
        return storage.DeleteAsync(alias, gameId);
    }

    public Task<GameStateMemory> LoadGameAsync(string alias, string gameId)
    {
        return storage.LoadAsync(alias, gameId);
    }

    public Task<IEnumerable<GameStateMemory>> LoadGamesAsync(string alias)
    {
        return storage.LoadAllAsync(alias);
    }

    public Task<GameStateMemory> ResetGameAsync(string alias, string gameId)
    {
        throw new NotImplementedException();
    }

    public Task SaveGameAsync(GameStateMemory gameState)
    {
        throw new NotImplementedException();
    }

    public Task<GameStateMemory> UndoGameAsync(string alias, string gameId)
    {
        throw new NotImplementedException();
    }
}