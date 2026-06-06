using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzlePoolService
{
    Task<int> GetAvailableCountAsync(GameDifficulty difficulty);
    Task SeedAsync(GameDifficulty difficulty, int count);
    Task<SudokuPuzzle?> DequeueAsync(GameDifficulty difficulty);
}
