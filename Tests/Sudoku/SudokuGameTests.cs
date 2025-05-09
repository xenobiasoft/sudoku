using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace UnitTests.Sudoku;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
    [Fact]
    public async Task DeleteAsync_DeletesGameStateFromStorage()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var mockGameStateManager = Container.ResolveMock<IGameStateStorage<GameStateMemory>>();
        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(puzzleId);

        // Assert
        mockGameStateManager.VerifyDeleteAsyncCalled(puzzleId, Times.Once);
    }

    [Fact]
    public async Task LoadAsync_LoadsGameStateFromGameStateManager()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var mockGameStateManager = Container.ResolveMock<IGameStateStorage<GameStateMemory>>();
        var sut = ResolveSut();

        // Act
        await sut.LoadAsync(puzzleId);

        // Assert
        mockGameStateManager.Verify(x => x.LoadAsync(puzzleId), Times.Once);
    }

    [Fact]
    public async Task NewGameAsync_GeneratesNewPuzzle()
    {
        // Arrange
        var mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>();
        var sut = ResolveSut();

        // Act
        await sut.NewGameAsync(Level.Easy);

        // Assert
        mockPuzzleGenerator.Verify(x => x.Generate(It.IsAny<Level>()), Times.Once);
    }

    [Fact]
    public async Task NewGameAsync_ReturnsGameState()
    {
        // Arrange
        Container.ResolveMock<IPuzzleGenerator>().SetupGenerate(PuzzleFactory.GetPuzzle(Level.Easy));
        var sut = ResolveSut();

        // Act
        var gameState = await sut.NewGameAsync(Level.Easy);

        // Assert
        gameState.VerifyNewGameState();
    }

    [Fact]
    public async Task SaveAsync_PersistsGameStateToStorage()
    {
        // Arrange
        var gameState = It.IsAny<GameStateMemory>();
        var mockStorage = Container.ResolveMock<IGameStateStorage<GameStateMemory>>();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState);

        // Assert
        mockStorage.VerifySaveAsyncCalled(gameState, Times.Once);
    }
}