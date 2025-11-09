using DepenMock.XUnit;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services;

public class StrategyBasedPuzzleSolverTests : BaseTestByAbstraction<StrategyBasedPuzzleSolver, IPuzzleSolver>
{
    private const string Alias = "SudokuSolverAlias";
    private readonly Mock<IPuzzleRepository> _puzzleRepository;

    public StrategyBasedPuzzleSolverTests()
    {
        _puzzleRepository = Container.ResolveMock<IPuzzleRepository>();
    }

    [Fact]
    public async Task SolvePuzzle_WhenInvalidMoveOccurs_TriggersUndo()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        _puzzleRepository.SaveAsyncThrows(new InvalidMoveException("Invalid move"));
        _puzzleRepository.UndoReturnsPuzzle(Alias, puzzle.PuzzleId, puzzle);
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(puzzle!);

        // Assert
        _puzzleRepository.Verify(x => x.UndoAsync(Alias, puzzle!.PuzzleId), Times.AtLeastOnce);
    }

    [Fact]
    public async Task SolvePuzzle_WhenPuzzleIsSolved_CallsSaveAsync()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(puzzle!);

        // Assert
        _puzzleRepository.VerifySaveAsync(Times.AtLeastOnce);
    }

    [Fact]
    public async Task SolvePuzzle_WhenPuzzleIsSolved_ReturnsSolvedPuzzle()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var sut = ResolveSut();

        // Act
        var result = await sut.SolvePuzzle(puzzle!);

        // Assert
        result.Should().NotBeNull();
        result.IsValid().Should().BeTrue();
        result.GetEmptyCellCount().Should().Be(0);
    }

    [Fact]
    public async Task SolvePuzzle_WhenUndoReturnsNull_ThrowsNoMoveHistoryException()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        _puzzleRepository.SaveAsyncThrows(new InvalidMoveException("Invalid move"));
        _puzzleRepository.UndoReturnsPuzzle(Alias, puzzle.PuzzleId, null);
        var sut = ResolveSut();

        // Act
        Task Solve() => sut.SolvePuzzle(puzzle!);

        // Assert
        await Assert.ThrowsAsync<NoMoveHistoryException>(Solve);
    }
}