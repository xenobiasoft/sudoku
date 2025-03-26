using DepenMock.XUnit;
using UnitTests.Helpers;
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
		puzzle.GetAllCells().Should().BeEquivalentTo(PuzzleFactory.GetEmptyPuzzle().GetAllCells());
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
		puzzleSolver.Verify(x => x.SolvePuzzle(It.IsAny<ISudokuPuzzle>()), Times.AtLeastOnce);
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
		puzzle.GetAllCells().Count(x => !x.Value.HasValue).Should().BeGreaterThanOrEqualTo(minEmptyCells).And.BeLessThanOrEqualTo(maxEmptyCells);
	}

	[Fact]
	public async Task Generate_WhenGeneratingPuzzle_MarksAllCellsWithValueAsLocked()
	{
        // Arrange
        var sut = ResolveSut();

        // Act
        var puzzle = await sut.Generate(Level.Easy);

        // Assert
        puzzle.GetAllCells().Where(x => x.Value.HasValue).ToList().ForEach(x => x.Locked.Should().BeTrue());
    }
}