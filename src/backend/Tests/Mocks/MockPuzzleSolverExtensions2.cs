using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using PuzzleFactory = UnitTests.Helpers.Factories.PuzzleFactory;

namespace UnitTests.Mocks;

public static class MockPuzzleSolverExtensions2
{
    extension(Mock<IPuzzleSolver> mock)
    {
        public Mock<IPuzzleSolver> ThrowInvalidPuzzleException()
        {
            mock
                .SetupSequence(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()))
                .Throws<InvalidPuzzleException>()
                .ReturnsAsync(PuzzleFactory.GetSolvedPuzzle);

            return mock;
        }

        public Mock<IPuzzleSolver> VerifyCallsSolvePuzzle(Func<Times> times)
        {
            mock.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), times);

            return mock;
        }

        public Mock<IPuzzleSolver> VerifyRetriesPuzzleGeneration()
        {
            mock.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Exactly(2));

            return mock;
        }
    }
}