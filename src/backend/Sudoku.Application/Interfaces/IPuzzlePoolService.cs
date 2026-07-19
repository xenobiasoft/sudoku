using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzlePoolService
{
    Task<int> GetAvailableCountAsync(BoardSize size, GameDifficulty difficulty);
    Task SeedAsync(BoardSize size, GameDifficulty difficulty, int count);
    Task<SudokuPuzzle?> DequeueAsync(BoardSize size, GameDifficulty difficulty);
}
