using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public interface IStorageService
{
    Task DeleteAsync(string puzzleId);
    Task<GameStateMemento> LoadAsync(string puzzleId);
    Task SaveAsync(string puzzleId, GameStateMemento gameState, CancellationToken token);
}