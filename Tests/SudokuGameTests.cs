using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;
using XenobiaSoft.Sudoku.Solver;

namespace UnitTests;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
	public SudokuGameTests()
	{
		Container
			.ResolveMock<IPuzzleGenerator>()
			.Setup(x => x.GenerateEmptyPuzzle())
			.ReturnsAsync(PuzzleFactory.GetEmptyPuzzle);
	}

	[Fact]
	public async Task New_GeneratesNewPuzzle()
	{
		// Arrange
		var mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>();
		var sut = ResolveSut();

		// Act
		await sut.New(Level.Easy);

		// Assert
		mockPuzzleGenerator.Verify(x => x.Generate(It.IsAny<Level>()), Times.Once);
	}

	[Fact]
	public async Task New_LoadsPuzzleThatWasGenerated()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		Container
			.ResolveMock<IPuzzleGenerator>()
			.Setup(x => x.Generate(It.IsAny<Level>()))
			.ReturnsAsync(puzzle);
		var sut = ResolveSut();

		// Act
		await sut.New(Level.Easy);

		// Assert
		sut.Puzzle.Should().BeEquivalentTo(puzzle);
	}

	[Fact]
	public async Task LoadPuzzle_SetsGamePuzzle()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
		var sut = ResolveSut();

		// Act
		await sut.LoadPuzzle(puzzle);

		// Assert
		Assert.Equivalent(puzzle, sut.Puzzle);
	}

	[Fact]
	public async Task Reset_GetsEmptyPuzzle_FromGenerator()
	{
		// Arrange
		var mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>();
		var sut = ResolveSut();

		// Act
		await sut.Reset();

		// Assert
		mockPuzzleGenerator.Verify(x => x.GenerateEmptyPuzzle(), Times.Once);
	}

	[Fact]
	public async Task SolvePuzzle_CallsPuzzleSolver()
	{
		// Arrange
		var mockPuzzleSolver = Container.ResolveMock<IPuzzleSolver>();
		var sut = ResolveSut();

		// Act
		await sut.SolvePuzzle();

		// Assert
		mockPuzzleSolver.Verify(x => x.SolvePuzzle(It.IsAny<Cell[]>()), Times.Once);
	}

	[Fact]
	public void SetCell_AcceptsValue_AndSetsCell()
	{
		// Arrange
		var sut = ResolveSut();
		sut.Reset();

		// Act
		sut.SetCell(2, 1, 5);

		// Assert
		sut.Puzzle.GetCell(2, 1).Value.Should().Be(5);
	}

	[Theory]
	[InlineData(5, 9, 6, "Invalid column (Parameter 'col')")]
	[InlineData(9, 5, 6, "Invalid row (Parameter 'row')")]
	[InlineData(1, 3, 10, "Invalid value (Parameter 'value')")]
	[InlineData(5, -1, 6, "Invalid column (Parameter 'col')")]
	[InlineData(-1, 5, 6, "Invalid row (Parameter 'row')")]
	[InlineData(1, 3, -1, "Invalid value (Parameter 'value')")]
	public void SetCell_WhenGivenInvalidNumber_ThrowsInvalidArgumentException(int row, int col, int value, string expectedMessage)
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		void SetCell() => sut.SetCell(row, col, value);

		// Assert
		Assert.Throws<ArgumentException>(SetCell).Message.Should().Be(expectedMessage);
	}
}