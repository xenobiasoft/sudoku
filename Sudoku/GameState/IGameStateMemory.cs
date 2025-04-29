namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemory
{
    GameStateMemoryType MemoryType { get; }
    Task ClearAsync(string puzzleId);
    Task<GameStateMemento?> LoadAsync(string puzzleId);
    Task SaveAsync(GameStateMemento gameState);
    Task<GameStateMemento?> UndoAsync(string puzzleId);
}

public enum GameStateMemoryType
{
    InMemory,
    AzureBlobPersistence
}