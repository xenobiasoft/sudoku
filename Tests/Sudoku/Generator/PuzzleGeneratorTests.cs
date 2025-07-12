using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;
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
		var puzzle = await sut.GenerateEmptyPuzzleAsync();

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
		await sut.GenerateAsync(GameDifficulty.Easy);

		// Assert
        puzzleSolver.VerifyCallsSolvePuzzle(Times.Once);
    }

	[Theory]
	[InlineData(GameDifficulty.Easy, 40, 45)]
	[InlineData(GameDifficulty.Medium, 46, 49)]
	[InlineData(GameDifficulty.Hard, 50, 53)]
	[InlineData(GameDifficulty.ExtremelyHard, 54, 58)]
	public async Task Generate_RemovesRandomCellsFromSolvedPuzzle(GameDifficulty difficulty, int minEmptyCells, int maxEmptyCells)
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		var puzzle = await sut.GenerateAsync(difficulty);

		// Assert
        puzzle.AssertHasExpectedNumberEmptyCells(minEmptyCells, maxEmptyCells);
    }

	[Fact]
	public async Task Generate_WhenGeneratingPuzzle_MarksAllCellsWithValueAsLocked()
	{
        // Arrange
        var sut = ResolveSut();

        // Act
        var puzzle = await sut.GenerateAsync(GameDifficulty.Easy);

        // Assert
        puzzle.AssertPopulatedCellsLocked();
    }

    [Fact]
    public async Task Generate_WhenSolverThrowsInvalidBoardException_RegeneratesBoard()
    {
        // Arrange
        var mockSolver = Container.ResolveMock<IPuzzleSolver>();
        mockSolver.ThrowInvalidBoardException();
        var sut = ResolveSut();

        // Act
        await sut.GenerateAsync(GameDifficulty.Easy);

        // Assert
        mockSolver.VerifyRetriesPuzzleGeneration();
    }
}