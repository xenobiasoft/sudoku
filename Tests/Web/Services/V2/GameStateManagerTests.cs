using DepenMock.XUnit;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;
using Sudoku.Web.Server.Services.HttpClients;
using Sudoku.Web.Server.Services.V2;

namespace UnitTests.Web.Services.V2;

public class GameStateManagerTests : BaseTestByAbstraction<GameManager, IGameStateManager>
{
    private const string TestAlias = "TestAlias";
    private const string TestGameId = "TestGameId";
    private const string TestDifficulty = "Easy";

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;
    private readonly Mock<IGameApiClient> _mockGameApiClient;

    public GameStateManagerTests()
    {
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
        _mockGameApiClient = Container.ResolveMock<IGameApiClient>();
    }

    [Fact]
    public async Task CreateGameAsync_WithValidParameters_CreatesGameSuccessfully()
    {
        // Arrange
        var expectedGame = CreateTestGameModel();
        var successResult = ApiResult<GameModel>.Success(expectedGame);
        _mockGameApiClient
            .Setup(x => x.CreateGameAsync(TestAlias, TestDifficulty))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var result = await sut.CreateGameAsync(TestAlias, TestDifficulty);

        // Assert
        result.Should().Be(expectedGame);
        GetGameProperty(sut).Should().Be(expectedGame);
    }

    [Fact]
    public async Task CreateGameAsync_WithValidParameters_SavesGameToLocalStorage()
    {
        // Arrange
        var expectedGame = CreateTestGameModel();
        var successResult = ApiResult<GameModel>.Success(expectedGame);
        _mockGameApiClient
            .Setup(x => x.CreateGameAsync(TestAlias, TestDifficulty))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        await sut.CreateGameAsync(TestAlias, TestDifficulty);

        // Assert
        _mockLocalStorageService.Verify(x => x.SaveGameStateAsync(expectedGame), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateGameAsync_WithEmptyOrNullAlias_ThrowsArgumentException(string alias)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateGameAsync(alias, TestDifficulty));

        // Assert
        exception.Message.Should().Be("Alias not set.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateGameAsync_WithEmptyOrNullDifficulty_ThrowsArgumentException(string difficulty)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateGameAsync(TestAlias, difficulty));

        // Assert
        exception.Message.Should().Be("Difficulty not set.");
    }

    [Fact]
    public async Task CreateGameAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        var failureResult = ApiResult<GameModel>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.CreateGameAsync(TestAlias, TestDifficulty))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.CreateGameAsync(TestAlias, TestDifficulty));

        // Assert
        exception.Message.Should().Be("Failed to create game.");
    }

    [Fact]
    public async Task CreateGameAsync_WhenApiReturnsNull_ThrowsException()
    {
        // Arrange
        var successResult = ApiResult<GameModel>.Success(null!);
        _mockGameApiClient
            .Setup(x => x.CreateGameAsync(TestAlias, TestDifficulty))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.CreateGameAsync(TestAlias, TestDifficulty));

        // Assert
        exception.Message.Should().Be("Failed to create game.");
    }

    [Fact]
    public async Task DeleteGameAsync_WithLoadedGame_DeletesSuccessfully()
    {
        // Arrange
        var game = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.DeleteGameAsync();

        // Assert
        GetGameProperty(sut).Should().BeNull();
    }

    [Fact]
    public async Task DeleteGameAsync_WithLoadedGame_CallsApiClient()
    {
        // Arrange
        var game = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.DeleteGameAsync();

        // Assert
        _mockGameApiClient.Verify(x => x.DeleteGameAsync(game.PlayerAlias, game.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WithLoadedGame_DeletesFromLocalStorage()
    {
        // Arrange
        var game = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.DeleteGameAsync();

        // Assert
        _mockLocalStorageService.Verify(x => x.DeleteGameAsync(game.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WithNoLoadedGame_ThrowsException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.DeleteGameAsync());

        // Assert
        exception.Message.Should().Be("No game loaded.");
    }

    [Fact]
    public async Task DeleteGameAsync_WithParameters_DeletesGameFromServer()
    {
        // Arrange
        var successResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(TestAlias, TestGameId);

        // Assert
        _mockGameApiClient.Verify(x => x.DeleteGameAsync(TestAlias, TestGameId), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_WithParameters_DeletesGameFromLocalStorage()
    {
        // Arrange
        var successResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(TestAlias, TestGameId);

        // Assert
        _mockLocalStorageService.Verify(x => x.DeleteGameAsync(TestGameId), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task DeleteGameAsync_WithEmptyOrNullAlias_ThrowsArgumentException(string alias)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.DeleteGameAsync(alias, TestGameId));

        // Assert
        exception.Message.Should().Be("Alias not set.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task DeleteGameAsync_WithEmptyOrNullGameId_ThrowsArgumentException(string gameId)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.DeleteGameAsync(TestAlias, gameId));

        // Assert
        exception.Message.Should().Be("Game ID not set.");
    }

    [Fact]
    public async Task DeleteGameAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        var failureResult = ApiResult<bool>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.DeleteGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.DeleteGameAsync(TestAlias, TestGameId));

        // Assert
        exception.Message.Should().Be("Failed to delete game from server.");
    }

    [Fact]
    public async Task LoadGameAsync_FromLocalStorage_ReturnsGameSuccessfully()
    {
        // Arrange
        var expectedGame = CreateTestGameModel();
        _mockLocalStorageService
            .Setup(x => x.LoadGameAsync(TestGameId))
            .ReturnsAsync(expectedGame);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(TestAlias, TestGameId);

        // Assert
        result.Should().Be(expectedGame);
        GetGameProperty(sut).Should().Be(expectedGame);
    }

    [Fact]
    public async Task LoadGameAsync_FromApi_WhenNotInLocalStorage_ReturnsGameSuccessfully()
    {
        // Arrange
        var expectedGame = CreateTestGameModel();
        _mockLocalStorageService
            .Setup(x => x.LoadGameAsync(TestGameId))
            .ReturnsAsync((GameModel)null!);
        var successResult = ApiResult<GameModel>.Success(expectedGame);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGameAsync(TestAlias, TestGameId);

        // Assert
        result.Should().Be(expectedGame);
        GetGameProperty(sut).Should().Be(expectedGame);
    }

    [Fact]
    public async Task LoadGameAsync_FromApi_SavesGameToLocalStorage()
    {
        // Arrange
        var expectedGame = CreateTestGameModel();
        _mockLocalStorageService
            .Setup(x => x.LoadGameAsync(TestGameId))
            .ReturnsAsync((GameModel)null!);
        var successResult = ApiResult<GameModel>.Success(expectedGame);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync(TestAlias, TestGameId);

        // Assert
        _mockLocalStorageService.Verify(x => x.SaveGameStateAsync(expectedGame), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task LoadGameAsync_WithEmptyOrNullAlias_ThrowsArgumentException(string alias)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.LoadGameAsync(alias, TestGameId));

        // Assert
        exception.Message.Should().Be("Alias not set.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task LoadGameAsync_WithEmptyOrNullGameId_ThrowsArgumentException(string gameId)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.LoadGameAsync(TestAlias, gameId));

        // Assert
        exception.Message.Should().Be("Game ID not set.");
    }

    [Fact]
    public async Task LoadGameAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        _mockLocalStorageService
            .Setup(x => x.LoadGameAsync(TestGameId))
            .ReturnsAsync((GameModel)null!);
        var failureResult = ApiResult<GameModel>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.LoadGameAsync(TestAlias, TestGameId));

        // Assert
        exception.Message.Should().Be("Failed to load game.");
    }

    [Fact]
    public async Task LoadGameAsync_WhenApiReturnsNull_ThrowsException()
    {
        // Arrange
        _mockLocalStorageService
            .Setup(x => x.LoadGameAsync(TestGameId))
            .ReturnsAsync((GameModel)null!);
        var successResult = ApiResult<GameModel>.Success(null!);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(TestAlias, TestGameId))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.LoadGameAsync(TestAlias, TestGameId));

        // Assert
        exception.Message.Should().Be("Failed to load game.");
    }

    [Fact]
    public async Task LoadGamesAsync_FromLocalStorage_ReturnsGamesSuccessfully()
    {
        // Arrange
        var expectedGames = new List<GameModel> { CreateTestGameModel() };
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync(expectedGames);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGamesAsync(TestAlias);

        // Assert
        result.Should().BeEquivalentTo(expectedGames);
    }

    [Fact]
    public async Task LoadGamesAsync_FromApi_WhenLocalStorageEmpty_ReturnsGamesSuccessfully()
    {
        // Arrange
        var expectedGames = new List<GameModel> { CreateTestGameModel() };
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync([]);
        var successResult = ApiResult<List<GameModel>>.Success(expectedGames);
        _mockGameApiClient
            .Setup(x => x.GetAllGamesAsync(TestAlias))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadGamesAsync(TestAlias);

        // Assert
        result.Should().BeEquivalentTo(expectedGames);
    }

    [Fact]
    public async Task LoadGamesAsync_FromApi_SavesGamesToLocalStorage()
    {
        // Arrange
        var expectedGames = new List<GameModel> { CreateTestGameModel() };
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync([]);
        var successResult = ApiResult<List<GameModel>>.Success(expectedGames);
        _mockGameApiClient
            .Setup(x => x.GetAllGamesAsync(TestAlias))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        await sut.LoadGamesAsync(TestAlias);

        // Assert
        _mockLocalStorageService.Verify(x => x.SaveGameStateAsync(expectedGames[0]), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task LoadGamesAsync_WithEmptyOrNullAlias_ThrowsArgumentException(string alias)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => sut.LoadGamesAsync(alias));

        // Assert
        exception.Message.Should().Be("Alias not set.");
    }

    [Fact]
    public async Task LoadGamesAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync([]);
        var failureResult = ApiResult<List<GameModel>>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.GetAllGamesAsync(TestAlias))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.LoadGamesAsync(TestAlias));

        // Assert
        exception.Message.Should().Be("Failed to load games.");
    }

    [Fact]
    public async Task LoadGamesAsync_WhenApiReturnsNull_ThrowsException()
    {
        // Arrange
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync([]);
        var successResult = ApiResult<List<GameModel>>.Success(null!);
        _mockGameApiClient
            .Setup(x => x.GetAllGamesAsync(TestAlias))
            .ReturnsAsync(successResult);
        var sut = ResolveSut();

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.LoadGamesAsync(TestAlias));

        // Assert
        exception.Message.Should().Be("Failed to load games.");
    }

    [Fact]
    public async Task ResetGameAsync_ResetsGameSuccessfully()
    {
        // Arrange
        var game = CreateTestGameModel();
        var resetGame = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        var gameResult = ApiResult<GameModel>.Success(resetGame);
        _mockGameApiClient
            .Setup(x => x.ResetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(gameResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        var result = await sut.ResetGameAsync();

        // Assert
        result.Should().Be(resetGame);
        GetGameProperty(sut).Should().Be(resetGame);
    }

    [Fact]
    public async Task ResetGameAsync_SavesResetGameToLocalStorage()
    {
        // Arrange
        var game = CreateTestGameModel();
        var resetGame = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        var gameResult = ApiResult<GameModel>.Success(resetGame);
        _mockGameApiClient
            .Setup(x => x.ResetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(gameResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.ResetGameAsync();

        // Assert
        _mockLocalStorageService.Verify(x => x.SaveGameStateAsync(resetGame), Times.Once);
    }

    [Fact]
    public async Task ResetGameAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        var game = CreateTestGameModel();
        var failureResult = ApiResult<bool>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.ResetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.ResetGameAsync());

        // Assert
        exception.Message.Should().Be("Failed to reset game.");
    }

    [Fact]
    public async Task ResetGameAsync_WithNullGame_ThrowsNullReferenceException()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => sut.ResetGameAsync());
    }

    [Fact]
    public async Task SaveGameAsync_SavesGameSuccessfully()
    {
        // Arrange
        var game = CreateTestGameModel();
        var apiSuccessResult = ApiResult<bool>.Success(true);
        _mockGameApiClient
            .Setup(x => x.SaveGameAsync(game))
            .ReturnsAsync(apiSuccessResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        await sut.SaveGameAsync();

        // Assert
        _mockGameApiClient.Verify(x => x.SaveGameAsync(game), Times.Once);
        _mockLocalStorageService.Verify(x => x.SaveGameStateAsync(game), Times.Once);
    }

    [Fact]
    public async Task UndoGameAsync_WithMovesAvailable_UndoesSuccessfully()
    {
        // Arrange
        var game = CreateTestGameModelWithMoves();
        var undoGame = CreateTestGameModel();
        var successResult = ApiResult<bool>.Success(true);
        var gameResult = ApiResult<GameModel>.Success(undoGame);
        _mockGameApiClient
            .Setup(x => x.UndoMoveAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(successResult);
        _mockGameApiClient
            .Setup(x => x.GetGameAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(gameResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        var result = await sut.UndoGameAsync();

        // Assert
        result.Should().Be(undoGame);
        GetGameProperty(sut).Should().Be(undoGame);
    }

    [Fact]
    public async Task UndoGameAsync_WithNoMoves_ReturnsCurrentGame()
    {
        // Arrange
        var game = CreateTestGameModel(); // No moves recorded
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act
        var result = await sut.UndoGameAsync();

        // Assert
        result.Should().Be(game);
        _mockGameApiClient.Verify(x => x.UndoMoveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UndoGameAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        var game = CreateTestGameModelWithMoves();
        var failureResult = ApiResult<bool>.Failure("API Error");
        _mockGameApiClient
            .Setup(x => x.UndoMoveAsync(game.PlayerAlias, game.Id))
            .ReturnsAsync(failureResult);
        var sut = ResolveSut();
        SetGameProperty(sut, game);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => sut.UndoGameAsync());
        exception.Message.Should().Be("Failed to undo move.");
    }

    [Fact]
    public async Task UndoGameAsync_WithNullGame_ThrowsNullReferenceException()
    {
        // Arrange
        var sut = ResolveSut();
        SetGameProperty(sut, null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => sut.UndoGameAsync());
    }

    private static GameModel CreateTestGameModel()
    {
        return new GameModel
        {
            Id = TestGameId,
            PlayerAlias = TestAlias,
            Difficulty = TestDifficulty,
            Status = "InProgress",
            Statistics = new GameStatisticsModel(),
            CreatedAt = DateTime.UtcNow,
            Cells = new List<CellModel>()
        };
    }

    private static GameModel CreateTestGameModelWithMoves()
    {
        var game = new GameModel
        {
            Id = TestGameId,
            PlayerAlias = TestAlias,
            Difficulty = TestDifficulty,
            Status = "InProgress",
            Statistics = new GameStatisticsModel(),
            CreatedAt = DateTime.UtcNow,
            Cells = new List<CellModel>()
        };
        
        // Record some moves to make TotalMoves > 0
        game.Statistics.RecordMove(true);
        game.Statistics.RecordMove(true);
        game.Statistics.RecordMove(false);
        return game;
    }

    private static GameModel? GetGameProperty(IGameStateManager gameManager)
    {
        var concreteGameManager = gameManager as GameManager;
        return concreteGameManager?.Game;
    }

    private static void SetGameProperty(IGameStateManager gameManager, GameModel? game)
    {
        if (gameManager is GameManager concreteGameManager)
        {
            concreteGameManager.GetType().GetProperty("Game")!.SetValue(concreteGameManager, game);
        }
    }
}