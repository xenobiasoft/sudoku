namespace XenobiaSoft.Sudoku.GameState;

public interface IGameStateMemoryPersistence
{
    Task ClearAsync(string puzzleId);
    Task<GameStateMemento?> LoadAsync(string puzzleId);
    Task SaveAsync(GameStateMemento gameState);
}