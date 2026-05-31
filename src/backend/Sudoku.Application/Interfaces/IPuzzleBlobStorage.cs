using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzleBlobStorage
{
    Task<SudokuPuzzle> CreateAsync(GameDifficulty difficulty);
    Task DeleteAsync(string prefix, string puzzleId);
    Task<IEnumerable<SudokuPuzzle>> LoadAllAsync(string prefix);
}
