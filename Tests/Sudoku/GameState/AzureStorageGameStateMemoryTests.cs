using DepenMock.XUnit;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.GameState;

public class AzureStorageGameStateMemoryTests : BaseTestByAbstraction<AzureStorageGameStateMemory, IGameStateMemoryPersistence>
{
    [Fact]
    public async Task ClearAsync_ShouldCallDeleteAsync()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var storageServiceSpy = Container.ResolveMock<IStorageService>();
        var sut = ResolveSut();

        // Act
        await sut.ClearAsync(puzzleId);

        // Assert
        storageServiceSpy.Verify(s => s.DeleteAsync(puzzleId), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_ShouldCallLoadAsync()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var storageServiceSpy = Container.ResolveMock<IStorageService>();
        var sut = ResolveSut();

        // Act
        await sut.LoadAsync(puzzleId);

        // Assert
        storageServiceSpy.Verify(s => s.LoadAsync(puzzleId), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnGameState()
    {
        // Arrange
        var expectedGameState = Container.Create<GameStateMemento>();
        Container.ResolveMock<IStorageService>()
            .Setup(s => s.LoadAsync(expectedGameState.PuzzleId))
            .ReturnsAsync(expectedGameState);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(expectedGameState.PuzzleId);

        // Assert
        result.Should().Be(expectedGameState);
    }

    [Fact]
    public async Task SaveAsync_ShouldDebounceAndCallSaveAsync()
    {
        // Arrange
        var storageServiceSpy = Container.ResolveMock<IStorageService>();
        var gameState = Container.Create<GameStateMemento>();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState);

        // Assert
        storageServiceSpy.Verify(s => s.SaveAsync(gameState.PuzzleId, gameState, It.IsAny<CancellationToken>()), Times.Once);
    }
}