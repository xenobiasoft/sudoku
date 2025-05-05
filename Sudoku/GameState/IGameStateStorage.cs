namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateStorage
{
    GameStateMemoryType MemoryType { get; }
    Task DeleteAsync(string puzzleId);
    Task<GameStateMemory?> LoadAsync(string puzzleId);
    Task SaveAsync(GameStateMemory gameState);
    Task<GameStateMemory?> UndoAsync(string puzzleId);
}

public enum GameStateMemoryType
{
    InMemory,
    AzureBlobPersistence
}