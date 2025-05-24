using System.Text.Json;
using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class LocalStorageServiceTests : BaseTestByAbstraction<LocalStorageService, ILocalStorageService>
{
    private readonly Mock<IJsRuntimeWrapper> _mockJsRuntime;

    public LocalStorageServiceTests()
    {
        _mockJsRuntime = Container.ResolveMock<IJsRuntimeWrapper>();

        _mockJsRuntime.SetupSavedGames([]);
    }

    [Fact]
    public async Task DeleteGameAsync_RemovesSpecifiedGame()
    {
        // Arrange
        var game1 = Container.Create<GameStateMemory>();
        var game2 = Container.Create<GameStateMemory>();
        _mockJsRuntime.SetupSavedGames([game1, game2]);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(game1.PuzzleId);

        // Assert
        _mockJsRuntime.VerifySavesGame(JsonSerializer.Serialize(new List<GameStateMemory> { game2 }), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGameDoesNotExist_DoesNotChangeStorage()
    {
        // Arrange
        var game = Container.Create<GameStateMemory>();
        _mockJsRuntime.SetupSavedGames([game]);
        var sut = ResolveSut();
        var nonExistentId = Container.Create<string>();

        // Act
        await sut.DeleteGameAsync(nonExistentId);

        // Assert
        _mockJsRuntime.VerifySavesGame(JsonSerializer.Serialize(new List<GameStateMemory>() { game }), Times.Once);
    }

    [Fact]
    public async Task GetAliasAsync_ReturnsAliasFromStorage()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.GetAliasAsync();

        // Assert
        _mockJsRuntime.VerifyGetsAlias(Times.Once);
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasNotSet_ReturnsNull()
    {
        // Arrange
        _mockJsRuntime.SetupAlias(null);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_FetchesAllGamesFromLocalStorage()
    {
        // Arrange
        var savedGames = Container.CreateMany<GameStateMemory>().ToList();
        _mockJsRuntime.SetupSavedGames(savedGames);
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync("gameId");

        // Assert
        _mockJsRuntime.VerifyLoadsSavedGames(Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_WhenGameExists_ReturnsGameState()
    {
        // Arrange
        var expectedGameState = Container.Create<GameStateMemory>();
        _mockJsRuntime.SetupSavedGames([expectedGameState]);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(expectedGameState.PuzzleId);

        // Assert
        result.Should().BeEquivalentTo(expectedGameState);
    }

    [Fact]
    public async Task LoadGameAsync_WhenGameDoesNotExist_ReturnsNull()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(puzzleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameStatesAsync_ReturnsEmptyList_WhenStorageIsEmpty()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGames([]);
        var sut = ResolveSut();
        
        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadGameStatesAsync_ReturnsGames_WhenStorageIsNotEmpty()
    {
        // Arrange
        var expectedGames = Container.CreateMany<GameStateMemory>().ToList();
        _mockJsRuntime.SetupSavedGames(expectedGames);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedGames);
    }

    [Fact]
    public async Task RemoveGameAsync_RemovesGameFromStorage()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        _mockJsRuntime.SetupSavedGames([gameState]);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(gameState.PuzzleId);

        // Assert
        _mockJsRuntime.VerifySavesGame(Times.Once);
    }

    [Fact]
    public async Task SaveGameStateAsync_WhenGameDoesNotExists_AddsGameToStorage()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(gameState);

        // Assert
        _mockJsRuntime.VerifySavesGame(Times.Once);
    }

    [Fact]
    public async Task SaveGameStateAsync_WhenGameExists_UpdatesGameInStorage()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        _mockJsRuntime.SetupSavedGames([gameState]);
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(gameState);

        // Assert
        _mockJsRuntime.VerifySavesGame(Times.Once);
    }

    [Fact]
    public async Task SetAliasAsync_SavesAliasToStorage()
    {
        // Arrange
        var alias = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.SetAliasAsync(alias);

        // Assert
        _mockJsRuntime.VerifySavesAlias(alias, Times.Once);
    }
}