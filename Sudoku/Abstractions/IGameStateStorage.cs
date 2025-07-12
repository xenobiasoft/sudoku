using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Abstractions;

public interface IGameStateStorage
{
    GameStateMemoryType MemoryType { get; }
    Task DeleteAsync(string alias, string puzzleId);
    Task<GameStateMemory> LoadAsync(string alias, string puzzleId);
    Task<IEnumerable<GameStateMemory>> LoadAllAsync(string alias);
    Task<GameStateMemory> ResetAsync(string alias, string puzzleId);
    Task SaveAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoAsync(string alias, string puzzleId);
}

public enum GameStateMemoryType
{
    InMemory,
    AzureBlobPersistence
}