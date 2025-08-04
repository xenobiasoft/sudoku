using DepenMock.XUnit;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using PuzzleFactory = UnitTests.Helpers.Factories.PuzzleFactory;

namespace UnitTests.Infrastructure.Services;

public class PuzzleGeneratorTests : BaseTestByAbstraction<PuzzleGenerator, IPuzzleGenerator>
{
    public PuzzleGeneratorTests()
    {
        var solvedPuzzle = PuzzleFactory.GetSolvedPuzzle();
        Container
            .ResolveMock<IPuzzleSolver>()
            .Setup(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()))
            .ReturnsAsync(solvedPuzzle);
    }

    [Theory]
    [InlineData("Easy", 40, 45)]
    [InlineData("Medium", 46, 49)]
    [InlineData("Hard", 50, 53)]
    [InlineData("Expert", 54, 58)]
    public async Task Generate_RemovesRandomCellsFromSolvedPuzzle(string difficulty, int minEmptyCells, int maxEmptyCells)
    {
        // Arrange
        var gameDifficulty = GameDifficulty.FromName(difficulty);
        var sut = ResolveSut();

        // Act
        var puzzle = await sut.GeneratePuzzleAsync(gameDifficulty);

        // Assert
        puzzle.AssertHasExpectedNumberEmptyCells(minEmptyCells, maxEmptyCells);
    }

    [Fact]
    public async Task Generate_SolvesEmptyPuzzle()
    {
        // Arrange
        var puzzleSolver = Container.ResolveMock<IPuzzleSolver>();
        var sut = ResolveSut();

        // Act
        await sut.GeneratePuzzleAsync(GameDifficulty.Easy);

        // Assert
        puzzleSolver.VerifyCallsSolvePuzzle(Times.Once);
    }

    [Fact]
    public async Task Generate_WhenGeneratingPuzzle_MarksAllCellsWithValueAsLocked()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var puzzle = await sut.GeneratePuzzleAsync(GameDifficulty.Easy);

        // Assert
        puzzle.AssertPopulatedCellsLocked();
    }

    [Fact]
    public async Task Generate_WhenSolverThrowsInvalidBoardException_RegeneratesBoard()
    {
        // Arrange
        var mockSolver = Container.ResolveMock<IPuzzleSolver>();
        mockSolver.ThrowInvalidPuzzleException();
        var sut = ResolveSut();

        // Act
        await sut.GeneratePuzzleAsync(GameDifficulty.Easy);

        // Assert
        mockSolver.VerifyRetriesPuzzleGeneration();
    }
}