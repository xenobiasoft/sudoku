using Sudoku.Domain.Entities;

namespace Sudoku.Application.Interfaces;

public interface IPuzzleSolver
{
	Task<SudokuPuzzle> SolvePuzzle(SudokuPuzzle puzzle);
}