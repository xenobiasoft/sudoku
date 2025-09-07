using XenobiaSoft.Sudoku;

namespace UnitTests.Mocks;

public static class MockSudokuPuzzleExtensions
{
    public static Mock<ISudokuPuzzle> SetupPuzzleNotInitiallySolved(this Mock<ISudokuPuzzle> mock)
    {
        mock
            .SetupSequence(x => x.IsSolved())
            .Returns(false)
            .Returns(true)
            .Returns(true);

        return mock;
    }

    public static Mock<ISudokuPuzzle> SetupPuzzleIsSolved(this Mock<ISudokuPuzzle> mock)
    {
        mock
            .Setup(x => x.IsSolved())
            .Returns(true);

        return mock;
    }

    public static Mock<ISudokuPuzzle> SetupInvalidMove(this Mock<ISudokuPuzzle> mock)
    {
        mock
            .SetupSequence(x => x.IsValid())
            .Returns(true)
            .Returns(false)
            .Returns(true);
        mock
            .SetupSequence(x => x.IsSolved())
            .Returns(false)
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