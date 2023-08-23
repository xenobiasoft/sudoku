using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public static class SudokuPuzzleExtensionMethods
{
    public static SudokuPuzzleAssertions Should(this SudokuPuzzle instance)
    {
        return new SudokuPuzzleAssertions(instance);
    }
}