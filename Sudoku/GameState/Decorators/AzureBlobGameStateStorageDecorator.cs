namespace XenobiaSoft.Sudoku.GameState.Decorators;

public abstract class AzureBlobGameStateStorageDecorator(IGameStateStorage<GameStateMemory> decorated) : IGameStateStorage<GameStateMemory>
{
    public GameStateMemoryType MemoryType => decorated.MemoryType;

    public abstract Task DeleteAsync(string puzzleId);
    public abstract Task<GameStateMemory> LoadAsync(string puzzleId);
    public abstract Task SaveAsync(GameStateMemory gameState);
    public abstract Task<GameStateMemory> UndoAsync(string puzzleId);
}