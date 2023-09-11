namespace XenobiaSoft.Sudoku.Generator;

public interface IPuzzleGenerator
{
	Task<Cell[]> Generate(Level level);
}