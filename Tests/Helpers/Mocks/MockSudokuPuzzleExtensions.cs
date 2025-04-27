using XenobiaSoft.Sudoku;

namespace UnitTests.Helpers.Mocks;

public static class MockSudokuPuzzleExtensions
{
    public static Mock<ISudokuPuzzle> ThrowsInvalidMoveThenPuzzleSolved(this Mock<ISudokuPuzzle> mock)
    {
        mock
            .SetupSequence(x => x.IsValid())
            .Returns(false)
            .Returns(true);
        mock
            .Setup(x => x.IsSolved())
            .Returns(true);

        return mock;
    }

    public static Mock<ISudokuPuzzle> VerifyUsedBruteForce(this Mock<ISudokuPuzzle> mock)
    {
        mock.Verify(p => p.PopulatePossibleValues(), Times.Once);
        mock.Verify(p => p.SetCellWithFewestPossibleValues(), Times.Once);

        return mock;
    }
}