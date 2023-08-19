using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.StrategyTests;

public class PuzzleSolverTests
{
    [Fact]
    public void SolvePuzzle_IfChangesWereMadeToPuzzle_ContinuesLoopingThroughStrategies()
    {
        // Arrange
        var solverStrategy = new Mock<SolverStrategy>();
        solverStrategy
	        .SetupSequence(x => x.Execute(It.IsAny<SudokuPuzzle>()))
	        .Returns(4)
	        .Returns(4)
	        .Returns(0);
        var strategies = new List<SolverStrategy>
        {
            solverStrategy.Object,
        };
        var puzzle = new SudokuPuzzle();
        var sut = new PuzzleSolver(strategies);

        // Act
        sut.SolvePuzzle(puzzle);

        // Assert
        solverStrategy.Verify(x => x.Execute(puzzle), Times.Exactly(3));
    }

    [Fact]
    public void SolvePuzzle_WhenStrategyReturnsScore_AddsToTotalScore()
    {
		// Arrange
		var expectedScore = 4;
		var solverStrategy = new Mock<SolverStrategy>();
		solverStrategy
			.SetupSequence(x => x.Execute(It.IsAny<SudokuPuzzle>()))
			.Returns(expectedScore)
			.Returns(0);
		var strategies = new List<SolverStrategy>
		{
			solverStrategy.Object,
		};
		var puzzle = new SudokuPuzzle();
		var sut = new PuzzleSolver(strategies);

		// Act
		sut.SolvePuzzle(puzzle);

		// Assert
		sut.TotalScore.Should().Be(4);
    }
}