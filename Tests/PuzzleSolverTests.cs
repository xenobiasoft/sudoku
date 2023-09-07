using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests;

public class PuzzleSolverTests : BaseTestByAbstraction<PuzzleSolver, IPuzzleSolver>
{
    [Fact]
    public void TrySolvePuzzle_IfChangesWereMadeToPuzzle_ContinuesLoopingThroughStrategies()
    {
        // Arrange
        var solverStrategy = Container.ResolveMock<SolverStrategy>();
        solverStrategy
            .SetupSequence(x => x.Execute(It.IsAny<SudokuPuzzle>()))
            .Returns(4)
            .Returns(4)
            .Returns(0);
        var sut = ResolveSut();

        // Act
        sut.TrySolvePuzzle(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));

        // Assert
        solverStrategy.Verify(x => x.Execute(It.IsAny<SudokuPuzzle>()), Times.Exactly(3));
    }

    [Fact]
    public void IsSolved_WhenPuzzleIsValidAndAllValuesPopulatedWithNumber_ReturnsTrue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var sut = ResolveSut();

        // Act
        var isSolved = sut.IsSolved(puzzle);

        // Assert
        isSolved.Should().BeTrue();
	}

    [Theory]
    [InlineData(Level.Easy)]
    [InlineData(Level.Medium)]
    [InlineData(Level.Hard)]
    [InlineData(Level.ExtremelyHard)]
	public void IsValid_WhenGivenValidPuzzle_ReturnsTrue(Level level)
    {
	    // Arrange
	    var puzzle = PuzzleFactory.GetPuzzle(level);
	    var sut = ResolveSut();

	    // Act
	    var isValid = sut.IsValid(puzzle);

	    // Assert
	    isValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenGivenEmptyPuzzle_ReturnsTrue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetEmptyPuzzle();
		var sut = ResolveSut();

		// Act
		var isValid = sut.IsValid(puzzle);

		// Assert
		isValid.Should().BeTrue();
	}

    [Fact]
    public void IsValid_WhenGivenCompletedValidPuzzle_ReturnsTrue()
    {
	    // Arrange
	    var puzzle = PuzzleFactory.GetSolvedPuzzle();
	    var sut = ResolveSut();

	    // Act
	    var isValid = sut.IsValid(puzzle);

	    // Assert
	    isValid.Should().BeTrue();
    }
}