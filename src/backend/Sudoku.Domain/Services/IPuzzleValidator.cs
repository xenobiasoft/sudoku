using Sudoku.Domain.Entities;

namespace Sudoku.Domain.Services;

public interface IPuzzleValidator
{
    bool IsValid(SudokuPuzzle puzzle);
    bool HasUniqueSolution(SudokuPuzzle puzzle);
    bool IsSolvable(SudokuPuzzle puzzle);
}