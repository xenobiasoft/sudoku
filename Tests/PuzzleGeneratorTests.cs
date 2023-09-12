using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests;

public class PuzzleGeneratorTests : BaseTestByAbstraction<PuzzleGenerator, IPuzzleGenerator>
{
	public PuzzleGeneratorTests()
	{
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.SolvePuzzle(It.IsAny<Cell[]>()))
			.ReturnsAsync(PuzzleFactory.GetSolvedPuzzle());
	}

	[Fact]
	public async Task GenerateEmptyPuzzle_ReturnsPuzzleWithAllCellsValueNull()
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		var puzzle = await sut.GenerateEmptyPuzzle();

		// Assert
		puzzle.Should().BeEquivalentTo(PuzzleFactory.GetEmptyPuzzle());
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
		puzzleSolver.Verify(x => x.SolvePuzzle(It.IsAny<Cell[]>()), Times.AtLeastOnce);
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
		puzzle.Count(x => !x.Value.HasValue).Should().BeGreaterThanOrEqualTo(minEmptyCells).And.BeLessOrEqualTo(maxEmptyCells);
	}
}