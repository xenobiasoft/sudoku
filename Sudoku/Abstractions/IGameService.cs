using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Abstractions;

public interface IGameService
{
    Task<string> CreateGameAsync(string alias, GameDifficulty difficulty);
    Task DeleteGameAsync(string alias, string gameId);
    Task<GameStateMemory?> LoadGameAsync(string alias, string gameId);
    Task<IEnumerable<GameStateMemory>> LoadGamesAsync(string alias);
    Task<GameStateMemory> ResetGameAsync(string alias, string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoGameAsync(string alias, string gameId);
}