using DepenMock.XUnit;
using UnitTests.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Exceptions;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku.Solver;

public class StrategyBasedPuzzleSolverTests : BaseTestByAbstraction<StrategyBasedPuzzleSolver, IPuzzleSolver>
{
    private const string Alias = "SudokuSolverAlias";

    [Fact]
    public async Task SolvePuzzle_SavesGameState_OnEachLoop()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupPuzzleIsSolved();
        var mockGameStateStorage = Container.ResolveMock<IInMemoryGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateStorage.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_ShouldUndoOnInvalidMove()
    {
        // Arrange
        var mockGameStateStorage = Container.ResolveMock<IInMemoryGameStateStorage>();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateStorage.VerifyUndoAsyncCalled(Alias, mockPuzzle.Object.PuzzleId, Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_ShouldUseBruteForceWhenStrategiesFail()
    {
        // Arrange
        Container
            .ResolveMock<SolverStrategy>()
            .DoesNotMakeChanges();
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupPuzzleNotInitiallySolved();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockPuzzle.VerifyUsedBruteForce();
    }

    [Fact]
    public async Task SolvePuzzle_WhenGameStateIsEmpty_AndUndoCalled_ThrowsInvalidBoardException()
    {
        // Arrange
        Container.ResolveMock<IInMemoryGameStateStorage>().SetupUndoAsync(null);
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var sut = ResolveSut();

        // Act
        Task SolvePuzzle() => sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        await Assert.ThrowsAsync<InvalidBoardException>(SolvePuzzle);
    }

    [Fact]
    public async Task SolvePuzzle_WhenPuzzleIsSolved_ClearsMemoryState()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupPuzzleIsSolved();
        var mockGameStateStorage = Container.ResolveMock<IInMemoryGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateStorage.VerifyDeleteAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SolvePuzzle_WhenStrategyMakesInvalidMove_GamePopsSavedStateOffStack()
    {
        // Arrange
        var mockPuzzle = Container.ResolveMock<ISudokuPuzzle>();
        mockPuzzle.SetupInvalidMove();
        var mockGameStateStorage = Container.ResolveMock<IInMemoryGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(mockPuzzle.Object);

        // Assert
        mockGameStateStorage.VerifyUndoAsyncCalled(Alias, mockPuzzle.Object.PuzzleId, Times.Once);
    }
}