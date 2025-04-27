using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests.Helpers.Mocks;

public static class MockPuzzleSolverExtensions
{
    public static Mock<IPuzzleSolver> VerifyCallsSolvePuzzle(this Mock<IPuzzleSolver> mock, Func<Times> times)
    {
        mock.Verify(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()), times);

        return mock;
    }
}