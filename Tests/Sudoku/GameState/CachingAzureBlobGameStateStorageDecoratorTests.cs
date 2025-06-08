using DepenMock.XUnit;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.GameState.Decorators;

namespace UnitTests.Sudoku.GameState;

public class CachingAzureBlobGameStateStorageDecoratorTests : BaseTestByAbstraction<CachingAzureBlobGameStateStorageDecorator, IPersistentGameStateStorage>
{
    private const string PuzzleId = "test-puzzle";
    private const string Alias = "test-alias";

    private readonly Mock<IPersistentGameStateStorage> _mockDecoratedStorage;
    private readonly GameStateMemory _gameState;

    public CachingAzureBlobGameStateStorageDecoratorTests()
    {
        _mockDecoratedStorage = Container.ResolveMock<IPersistentGameStateStorage>();
        _gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Board, () => Container.CreateMany<Cell>(40).ToArray())
            .Create();
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDecorated_DeleteAsync()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.SaveAsync(_gameState);

        // Act
        await sut.DeleteAsync(Alias, PuzzleId);

        // Assert
        _mockDecoratedStorage.VerifyDeleteAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task LoadAsync_WhenAlreadyCached_ShouldReturnCachedGameState()
    {
        // Arrange
        var sut = ResolveSut();
        await sut.SaveAsync(_gameState);

        // Act
        var result = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        result.Should().Be(_gameState);
        _mockDecoratedStorage.Verify(x => x.LoadAsync(Alias, PuzzleId), Times.Never);
    }

    [Fact]
    public async Task LoadAsync_WhenNotCached_ShouldCallDecoratedStorage()
    {
        // Arrange
        _mockDecoratedStorage.SetupLoadAsync(_gameState);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        result.Should().Be(_gameState);
        _mockDecoratedStorage.Verify(x => x.LoadAsync(Alias, PuzzleId), Times.Once);
    }

    [Fact]
    public async Task ResetAsync_ShouldCallDecorated_ResetAsync()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(Alias, PuzzleId);

        // Assert
        _mockDecoratedStorage.VerifyResetAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_ShouldUpdateCache()
    {
        // Arrange
        _mockDecoratedStorage.SetupResetAsync(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(Alias, PuzzleId);

        // Assert
        var cachedGameState = await sut.LoadAsync(Alias, PuzzleId);
        cachedGameState!.AssertAreEquivalent(_gameState);
    }

    [Fact]
    public async Task SaveAsync_ShouldCacheGameState()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        var cachedGameState = await sut.LoadAsync(Alias, PuzzleId);
        cachedGameState.Should().Be(_gameState);
        _mockDecoratedStorage.VerifySaveAsyncCalled(_gameState, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_ShouldCallDecorated_UndoAsync()
    {
        // Arrange
        _mockDecoratedStorage.SetupUndoAsync(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(Alias, PuzzleId);

        // Assert
        _mockDecoratedStorage.VerifyUndoAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_ShouldUpdateCache()
    {
        // Arrange
        _mockDecoratedStorage.SetupUndoAsync(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(Alias, PuzzleId);

        // Assert
        var cachedGameState = await sut.LoadAsync(Alias, PuzzleId);
        cachedGameState!.AssertAreEquivalent(_gameState);
    }
}
