using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Domain.Repositories;

public interface IPuzzleRepository
{
    Task<SudokuPuzzle?> GetByIdAsync(string puzzleId);
    Task<SudokuPuzzle?> GetByDifficultyAsync(GameDifficulty difficulty);
    Task<IEnumerable<SudokuPuzzle>> GetAllAsync();
    Task SaveAsync(SudokuPuzzle puzzle);
    Task DeleteAsync(string puzzleId);
    Task<bool> ExistsAsync(string puzzleId);
}