using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameStateManager
{
    Task DeleteGameAsync(string alias, string gameId);
    Task<string> GetGameAliasAsync();
    Task<GameStateMemory?> LoadGameAsync(string alias, string gameId);
    Task<List<GameStateMemory>> LoadGamesAsync();
    Task<GameStateMemory> ResetGameAsync(string alias, string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoGameAsync(string alias, string gameId);
}