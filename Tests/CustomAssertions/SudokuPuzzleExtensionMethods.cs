using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public static class SudokuPuzzleExtensionMethods
{
    public static SudokuPuzzleAssertions Should(this Cell[] instance)
    {
        return new SudokuPuzzleAssertions(instance);
    }
}