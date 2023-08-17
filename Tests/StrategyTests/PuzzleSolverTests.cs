using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class PuzzleSolverTests
{
    [Fact]
    public void PuzzleSolver_WhenGivenPuzzle_DelegatesSolvingToStrategies()
    {
        // Arrange
        var solverStrategy = new Mock<SolverStrategy>();
        var strategies = new List<SolverStrategy>
        {
            solverStrategy.Object,
        };
        var puzzle = new SudokuPuzzle();
        var sut = new PuzzleSolver(strategies);

        // Act
        sut.SolvePuzzle(puzzle);

        // Assert
        solverStrategy.Verify(x => x.Execute(puzzle), Times.Once);
    }
}