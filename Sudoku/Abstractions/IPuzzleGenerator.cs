namespace XenobiaSoft.Sudoku.Abstractions;

public interface IPuzzleGenerator
{
	Task<ISudokuPuzzle> Generate(Level level);
	Task<ISudokuPuzzle> GenerateEmptyPuzzle();
}