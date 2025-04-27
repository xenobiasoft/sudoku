namespace XenobiaSoft.Sudoku.GameState;

public interface IPersistenceGameStateMemory
{
    Task ClearAsync(string puzzleId);
    Task<GameStateMemento?> LoadAsync(string puzzleId);
    Task SaveAsync(GameStateMemento gameState);
    Task<GameStateMemento?> UndoAsync(string puzzleId);
}