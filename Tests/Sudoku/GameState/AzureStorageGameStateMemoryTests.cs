using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.GameState;

public class AzureStorageGameStateMemoryTests : BaseTestByAbstraction<AzureStorageGameStateMemory, IGameStateMemoryPersistence>
{
    private const string ContainerName = "sudoku-puzzles";
    private const string BlobName = "game-state.json";

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
        storageServiceSpy.VerifyDeletesBlob(ContainerName, GetBlobName(puzzleId), Times.Once);
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
        storageServiceSpy.VerifyLoadsGameState(ContainerName, GetBlobName(puzzleId), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnGameState()
    {
        // Arrange
        var expectedGameState = Container.Create<GameStateMemento>();
        Container.ResolveMock<IStorageService>()
            .Setup(s => s.LoadAsync<GameStateMemento>(ContainerName, GetBlobName(expectedGameState.PuzzleId)))
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
        storageServiceSpy.VerifySavesGameState(ContainerName, GetBlobName(gameState.PuzzleId), gameState, Times.Once);
    }

    private string GetBlobName(string puzzleId)
    {
        return $"{puzzleId}/{BlobName}";
    }
}