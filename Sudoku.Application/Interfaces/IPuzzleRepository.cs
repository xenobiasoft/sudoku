using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzleRepository
{
    Task<SudokuPuzzle?> GetByIdAsync(string puzzleId);
    Task<SudokuPuzzle> GetRandomByDifficultyAsync(GameDifficulty difficulty);
    Task SaveAsync(SudokuPuzzle puzzle);
}