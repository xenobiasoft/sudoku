namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateStorage<TMemoryStateType> where TMemoryStateType : PuzzleState
{
    GameStateMemoryType MemoryType { get; }
    Task DeleteAsync(string puzzleId);
    Task<TMemoryStateType?> LoadAsync(string puzzleId);
    Task SaveAsync(TMemoryStateType gameState);
    Task<TMemoryStateType?> UndoAsync(string puzzleId);
}

public enum GameStateMemoryType
{
    InMemory,
    AzureBlobPersistence
}