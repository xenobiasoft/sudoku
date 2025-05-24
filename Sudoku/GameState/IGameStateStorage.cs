namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateStorage<TMemoryStateType> where TMemoryStateType : PuzzleState
{
    GameStateMemoryType MemoryType { get; }
    Task DeleteAsync(string alias, string puzzleId);
    Task<TMemoryStateType?> LoadAsync(string alias, string puzzleId);
    Task<TMemoryStateType?> ResetAsync(string alias, string puzzleId);
    Task SaveAsync(TMemoryStateType gameState);
    Task<TMemoryStateType?> UndoAsync(string alias, string puzzleId);
}

public enum GameStateMemoryType
{
    InMemory,
    AzureBlobPersistence
}