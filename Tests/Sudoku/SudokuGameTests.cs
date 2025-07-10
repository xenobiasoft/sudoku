using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Sudoku;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
    private const string Alias = "testAlias";
    private const string PuzzleId = "testPuzzleId";

    [Fact]
    public async Task DeleteAsync_DeletesGameStateFromStorage()
    {
        // Arrange
        var mockGameStateManager = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(Alias, PuzzleId);

        // Assert
        mockGameStateManager.VerifyDeleteAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task LoadAsync_LoadsGameStateFromGameStateManager()
    {
        // Arrange
        var mockGameStateManager = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        mockGameStateManager.Verify(x => x.LoadAsync(Alias, PuzzleId), Times.Once);
    }

    [Fact]
    public async Task NewGameAsync_GeneratesNewPuzzle()
    {
        // Arrange
        var mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>();
        var sut = ResolveSut();

        // Act
        await sut.NewGameAsync(Alias, Level.Easy);

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
        var gameState = await sut.NewGameAsync(Alias, Level.Easy);

        // Assert
        gameState.VerifyNewGameState();
    }

    [Fact]
    public async Task SaveAsync_PersistsGameStateToStorage()
    {
        // Arrange
        var gameState = It.IsAny<GameStateMemory>();
        var mockStorage = Container.ResolveMock<IGameStateStorage>();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState);

        // Assert
        mockStorage.VerifySaveAsyncCalled(gameState, Times.Once);
    }
}