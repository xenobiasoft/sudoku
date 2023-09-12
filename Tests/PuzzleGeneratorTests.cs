using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests;

public class PuzzleGeneratorTests : BaseTestByAbstraction<PuzzleGenerator, IPuzzleGenerator>
{
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
}