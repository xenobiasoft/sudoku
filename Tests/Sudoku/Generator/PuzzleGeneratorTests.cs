using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests.Sudoku.Generator;

public class PuzzleGeneratorTests : BaseTestByAbstraction<PuzzleGenerator, IPuzzleGenerator>
{
	public PuzzleGeneratorTests()
    {
        var solvedPuzzle = PuzzleFactory.GetSolvedPuzzle();
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()))
			.ReturnsAsync(solvedPuzzle);
	}

	[Fact]
	public async Task GenerateEmptyPuzzle_ReturnsPuzzleWithAllCellsValueNull()
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		var puzzle = await sut.GenerateEmptyPuzzle();

		// Assert
        puzzle.AssertAllCellsEmpty();
    }

	[Fact]
	public async Task Generate_SolvesEmptyPuzzle()
	{
		// Arrange
		var puzzleSolver = Container.ResolveMock<IPuzzleSolver>();
		var sut = ResolveSut();

		// Act
		await sut.Generate(Level.Easy);

		// Assert
        puzzleSolver.VerifyCallsSolvePuzzle(Times.Once);
    }

	[Theory]
	[InlineData(Level.Easy, 40, 45)]
	[InlineData(Level.Medium, 46, 49)]
	[InlineData(Level.Hard, 50, 53)]
	[InlineData(Level.ExtremelyHard, 54, 58)]
	public async Task Generate_RemovesRandomCellsFromSolvedPuzzle(Level level, int minEmptyCells, int maxEmptyCells)
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		var puzzle = await sut.Generate(level);

		// Assert
        puzzle.AssertHasExpectedNumberEmptyCells(minEmptyCells, maxEmptyCells);
    }

	[Fact]
	public async Task Generate_WhenGeneratingPuzzle_MarksAllCellsWithValueAsLocked()
	{
        // Arrange
        var sut = ResolveSut();

        // Act
        var puzzle = await sut.Generate(Level.Easy);

        // Assert
        puzzle.AssertPopulatedCellsLocked();
    }
}