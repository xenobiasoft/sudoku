using DepenMock.XUnit;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.V2;
using System.Text.Json;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.V2.ILocalStorageService;

namespace UnitTests.Web.Services.V2;

public class LocalStorageServiceTests : BaseTestByAbstraction<LocalStorageService, ILocalStorageService>
{
    private readonly Mock<IJsRuntimeWrapper> _mockJsRuntime;

    public LocalStorageServiceTests()
    {
        _mockJsRuntime = Container.ResolveMock<IJsRuntimeWrapper>();
    }

    #region DeleteGameAsync Tests

    [Fact]
    public async Task DeleteGameAsync_WithExistingGame_RemovesGameFromStorage()
    {
        // Arrange
        var gameToDelete = Container.Create<GameModel>();
        var gameToKeep = Container.Create<GameModel>();
        var games = new List<GameModel> { gameToDelete, gameToKeep };
        
        _mockJsRuntime.SetupSavedGamesV2(games);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(gameToDelete.Id);

        // Assert
        var expectedJson = JsonSerializer.Serialize(new List<GameModel> { gameToKeep });
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WithNonExistentGame_DoesNotModifyStorage()
    {
        // Arrange
        var existingGame = Container.Create<GameModel>();
        var games = new List<GameModel> { existingGame };
        var nonExistentGameId = Container.Create<string>();
        
        _mockJsRuntime.SetupSavedGamesV2(games);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(nonExistentGameId);

        // Assert
        var expectedJson = JsonSerializer.Serialize(games);
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WithEmptyStorage_DoesNotThrow()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();
        var gameId = Container.Create<string>();

        // Act & Assert
        await sut.DeleteGameAsync(gameId);
        
        var expectedJson = JsonSerializer.Serialize(new List<GameModel>());
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_LoadsGamesFromStorage()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();
        var gameId = Container.Create<string>();

        // Act
        await sut.DeleteGameAsync(gameId);

        // Assert
        _mockJsRuntime.VerifyLoadsSavedGamesV2(Times.Once);
    }

    #endregion

    #region GetAliasAsync Tests

    [Fact]
    public async Task GetAliasAsync_WhenAliasExists_ReturnsAlias()
    {
        // Arrange
        var expectedAlias = Container.Create<string>();
        _mockJsRuntime.SetupAliasV2(expectedAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().Be(expectedAlias);
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasIsNull_ReturnsNull()
    {
        // Arrange
        _mockJsRuntime.SetupAliasV2(null);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAliasAsync_CallsJsRuntimeWithCorrectKey()
    {
        // Arrange
        _mockJsRuntime.SetupAliasV2("test");
        var sut = ResolveSut();

        // Act
        await sut.GetAliasAsync();

        // Assert
        _mockJsRuntime.VerifyGetsAliasV2(Times.Once);
    }

    #endregion

    #region LoadGameAsync Tests

    [Fact]
    public async Task LoadGameAsync_WithExistingGame_ReturnsGame()
    {
        // Arrange
        var targetGame = Container.Create<GameModel>();
        var otherGame = Container.Create<GameModel>();
        var games = new List<GameModel> { targetGame, otherGame };
        
        _mockJsRuntime.SetupSavedGamesV2(games);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(targetGame.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(targetGame.Id);
    }

    [Fact]
    public async Task LoadGameAsync_WithNonExistentGame_ReturnsNull()
    {
        // Arrange
        var existingGame = Container.Create<GameModel>();
        var games = new List<GameModel> { existingGame };
        var nonExistentGameId = Container.Create<string>();
        
        _mockJsRuntime.SetupSavedGamesV2(games);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(nonExistentGameId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_WithEmptyStorage_ReturnsNull()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();
        var gameId = Container.Create<string>();

        // Act
        var result = await sut.LoadGameAsync(gameId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_LoadsGamesFromStorage()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();
        var gameId = Container.Create<string>();

        // Act
        await sut.LoadGameAsync(gameId);

        // Assert
        _mockJsRuntime.VerifyLoadsSavedGamesV2(Times.Once);
    }

    #endregion

    #region LoadGameStatesAsync Tests

    [Fact]
    public async Task LoadGameStatesAsync_WithValidJson_ReturnsGamesList()
    {
        // Arrange
        var expectedGames = Container.CreateMany<GameModel>(3).ToList();
        _mockJsRuntime.SetupSavedGamesV2(expectedGames);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedGames);
    }

    [Fact]
    public async Task LoadGameStatesAsync_WithEmptyStorage_ReturnsEmptyList()
    {
        // Arrange
        _mockJsRuntime.SetupEmptyStorage();
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadGameStatesAsync_WithWhitespaceJson_ReturnsEmptyList()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "savedGames")))
            .ReturnsAsync("   ");
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadGameStatesAsync_WithNullJson_ReturnsEmptyList()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "savedGames")))
            .ReturnsAsync((string)null!);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadGameStatesAsync_WithInvalidJson_ReturnsEmptyList()
    {
        // Arrange
        _mockJsRuntime
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "savedGames")))
            .ReturnsAsync("invalid json");
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameStatesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadGameStatesAsync_CallsJsRuntimeWithCorrectKey()
    {
        // Arrange
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();

        // Act
        await sut.LoadGameStatesAsync();

        // Assert
        _mockJsRuntime.VerifyLoadsSavedGamesV2(Times.Once);
    }

    #endregion

    #region SaveGameStateAsync Tests

    [Fact]
    public async Task SaveGameStateAsync_WithNewGame_AddsGameToStorage()
    {
        // Arrange
        var existingGames = Container.CreateMany<GameModel>(2).ToList();
        var newGame = Container.Create<GameModel>();
        
        _mockJsRuntime.SetupSavedGamesV2(existingGames);
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(newGame);

        // Assert
        var expectedGames = existingGames.Concat(new[] { newGame }).ToList();
        var expectedJson = JsonSerializer.Serialize(expectedGames);
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task SaveGameStateAsync_WithExistingGame_UpdatesGameInStorage()
    {
        // Arrange
        var existingGame = Container.Create<GameModel>();
        var otherGame = Container.Create<GameModel>();
        var existingGames = new List<GameModel> { existingGame, otherGame };
        
        var updatedGame = Container.Create<GameModel>();
        updatedGame.Id = existingGame.Id; // Same ID to trigger update
        
        _mockJsRuntime.SetupSavedGamesV2(existingGames);
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(updatedGame);

        // Assert
        var expectedGames = new List<GameModel> { otherGame, updatedGame };
        var expectedJson = JsonSerializer.Serialize(expectedGames);
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task SaveGameStateAsync_WithEmptyStorage_AddsFirstGame()
    {
        // Arrange
        var newGame = Container.Create<GameModel>();
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(newGame);

        // Assert
        var expectedGames = new List<GameModel> { newGame };
        var expectedJson = JsonSerializer.Serialize(expectedGames);
        _mockJsRuntime.VerifySavesGameV2(expectedJson, Times.Once);
    }

    [Fact]
    public async Task SaveGameStateAsync_LoadsExistingGamesFirst()
    {
        // Arrange
        var game = Container.Create<GameModel>();
        _mockJsRuntime.SetupSavedGamesV2(new List<GameModel>());
        var sut = ResolveSut();

        // Act
        await sut.SaveGameStateAsync(game);

        // Assert
        _mockJsRuntime.VerifyLoadsSavedGamesV2(Times.Once);
    }

    #endregion

    #region SetAliasAsync Tests

    [Fact]
    public async Task SetAliasAsync_CallsJsRuntimeWithCorrectParameters()
    {
        // Arrange
        var alias = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.SetAliasAsync(alias);

        // Assert
        _mockJsRuntime.VerifySavesAliasV2(alias, Times.Once);
    }

    [Fact]
    public async Task SetAliasAsync_WithEmptyAlias_StillCallsJsRuntime()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var sut = ResolveSut();

        // Act
        await sut.SetAliasAsync(emptyAlias);

        // Assert
        _mockJsRuntime.VerifySavesAliasV2(emptyAlias, Times.Once);
    }

    [Fact]
    public async Task SetAliasAsync_WithNullAlias_StillCallsJsRuntime()
    {
        // Arrange
        string nullAlias = null!;
        var sut = ResolveSut();

        // Act
        await sut.SetAliasAsync(nullAlias);

        // Assert
        _mockJsRuntime.VerifySavesAliasV2(nullAlias, Times.Once);
    }

    #endregion
}