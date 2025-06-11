using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Storage.Azure.GameState.Decorators;

public abstract class AzureBlobGameStateStorageDecorator(IPersistentGameStateStorage decorated) : IPersistentGameStateStorage
{
    public GameStateMemoryType MemoryType => decorated.MemoryType;

    public abstract Task DeleteAsync(string alias, string puzzleId);
    public abstract Task<GameStateMemory> LoadAsync(string alias, string puzzleId);

    public abstract Task<GameStateMemory> ResetAsync(string alias, string puzzleId);

    public abstract Task SaveAsync(GameStateMemory gameState);

    public abstract Task<GameStateMemory> UndoAsync(string alias, string puzzleId);
}