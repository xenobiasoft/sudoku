namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemory
{
    Task ClearAsync(string puzzleId);
    Task<GameStateMemento?> LoadAsync(string puzzleId);
    Task SaveAsync(GameStateMemento gameState);
    Task<GameStateMemento?> UndoAsync(string puzzleId);
}