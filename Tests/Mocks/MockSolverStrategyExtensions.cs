using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Mocks;

public static class MockSolverStrategyExtensions
{
    public static Mock<SolverStrategy> DoesNotMakeChanges(this Mock<SolverStrategy> mock)
    {
        mock.Setup(s => s.SolvePuzzle(It.IsAny<ISudokuPuzzle>())).Returns(false);

        return mock;
    }
}