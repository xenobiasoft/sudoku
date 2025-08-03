using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests.Helpers.Mocks;

public static class MockPuzzleSolverExtensions
{
    public static Mock<IPuzzleSolver> ThrowInvalidBoardException(this Mock<IPuzzleSolver> mock)
    {
        mock
            .SetupSequence(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()))
            .Throws<InvalidBoardException>()
            .ReturnsAsync(PuzzleFactory.GetSolvedPuzzle());

        return mock;
    }

    public static Mock<IPuzzleSolver> VerifyCallsSolvePuzzle(this Mock<IPuzzleSolver> mock, Func<Times> times)
    {
        mock.Verify(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()), times);

        return mock;
    }

    public static Mock<IPuzzleSolver> VerifyRetriesPuzzleGeneration(this Mock<IPuzzleSolver> mock)
    {
        mock.Verify(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()), Times.Exactly(2));

        return mock;
    }
}