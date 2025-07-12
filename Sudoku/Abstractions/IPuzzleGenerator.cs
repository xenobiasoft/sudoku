namespace XenobiaSoft.Sudoku.Abstractions;

public interface IPuzzleGenerator
{
	Task<ISudokuPuzzle> GenerateAsync(GameDifficulty difficulty);
	Task<ISudokuPuzzle> GenerateEmptyPuzzleAsync();
}