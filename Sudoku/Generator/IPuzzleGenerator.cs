namespace XenobiaSoft.Sudoku.Generator;

public interface IPuzzleGenerator
{
	Task<ISudokuPuzzle> Generate(Level level);
	Task<ISudokuPuzzle> GenerateEmptyPuzzle();
}