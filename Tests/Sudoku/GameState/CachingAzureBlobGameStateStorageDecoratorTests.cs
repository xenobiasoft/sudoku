using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.GameState.Decorators;

namespace UnitTests.Sudoku.GameState;

public class CachingAzureBlobGameStateStorageDecoratorTests : BaseTestByAbstraction<CachingAzureBlobGameStateStorageDecorator, IGameStateStorage<GameStateMemory>>
{
    private const string PuzzleId = "test-puzzle";

    private readonly Mock<IGameStateStorage<GameStateMemory>> _mockDecoratedStorage;
    private readonly GameStateMemory _gameState;

    public CachingAzureBlobGameStateStorageDecoratorTests()
    {
        _mockDecoratedStorage = Container.ResolveMock<IGameStateStorage<GameStateMemory>>();
        _gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Board, () => Container.CreateMany<Cell>(40).ToArray())
            .Create();
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnCachedGameState_WhenAlreadyCached()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.SaveAsync(_gameState);

        // Act
        var result = await sut.LoadAsync(PuzzleId);

        // Assert
        result.Should().Be(_gameState);
        _mockDecoratedStorage.Verify(x => x.LoadAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoadAsync_ShouldCallDecoratedStorage_WhenNotCached()
    {
        // Arrange
        _mockDecoratedStorage.SetupLoadAsync(_gameState);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(PuzzleId);

        // Assert
        result.Should().Be(_gameState);
        _mockDecoratedStorage.Verify(x => x.LoadAsync(PuzzleId), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ShouldCacheGameState()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        var cachedGameState = await sut.LoadAsync(PuzzleId);
        cachedGameState.Should().Be(_gameState);
        _mockDecoratedStorage.VerifySaveAsyncCalled(_gameState, Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsDecorated_DeleteAsync()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.SaveAsync(_gameState);

        // Act
        await sut.DeleteAsync(PuzzleId);

        // Assert
        _mockDecoratedStorage.VerifyDeleteAsyncCalled(PuzzleId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_CallsDecorated_UndoAsync()
    {
        // Arrange
        _mockDecoratedStorage.SetupUndoAsync(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(PuzzleId);

        // Assert
        _mockDecoratedStorage.VerifyUndoAsyncCalled(PuzzleId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_ShouldUpdateCache()
    {
        // Arrange
        _mockDecoratedStorage.SetupUndoAsync(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(PuzzleId);

        // Assert
        var cachedGameState = await sut.LoadAsync(PuzzleId);
        cachedGameState.AssertAreEquivalent(_gameState);
    }
}
