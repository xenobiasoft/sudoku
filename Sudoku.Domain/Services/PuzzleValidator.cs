using Sudoku.Domain.Entities;

namespace Sudoku.Domain.Services;

public class PuzzleValidator : IPuzzleValidator
{
    public bool IsValid(SudokuPuzzle puzzle)
    {
        return puzzle.IsValid();
    }

    public bool HasUniqueSolution(SudokuPuzzle puzzle)
    {
        // This is a simplified implementation
        // In a real application, you'd want a more sophisticated solver
        return puzzle.HasUniqueSolution();
    }

    public bool IsSolvable(SudokuPuzzle puzzle)
    {
        // A puzzle is solvable if it's valid and has a unique solution
        return IsValid(puzzle) && HasUniqueSolution(puzzle);
    }
}