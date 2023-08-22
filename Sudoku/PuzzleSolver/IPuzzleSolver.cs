namespace XenobiaSoft.Sudoku.PuzzleSolver;

public interface IPuzzleSolver
{
	int TrySolvePuzzle(SudokuPuzzle puzzle);
	bool IsSolved(SudokuPuzzle puzzle);
	bool IsValid(SudokuPuzzle puzzle);
}