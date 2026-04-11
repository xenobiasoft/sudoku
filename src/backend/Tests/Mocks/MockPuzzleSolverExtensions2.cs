using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using PuzzleFactory = UnitTests.Helpers.Factories.PuzzleFactory;

namespace UnitTests.Mocks;

public static class MockPuzzleSolverExtensions2
{
    public static Mock<IPuzzleSolver> ThrowInvalidPuzzleException(this Mock<IPuzzleSolver> mock)
    {
        mock
            .SetupSequence(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()))
            .Throws<InvalidPuzzleException>()
            .ReturnsAsync(PuzzleFactory.GetSolvedPuzzle);

        return mock;
    }

    public static Mock<IPuzzleSolver> VerifyCallsSolvePuzzle(this Mock<IPuzzleSolver> mock, Func<Times> times)
    {
        mock.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), times);

        return mock;
    }

    public static Mock<IPuzzleSolver> VerifyRetriesPuzzleGeneration(this Mock<IPuzzleSolver> mock)
    {
        mock.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Exactly(2));

        return mock;
    }
}