using DepenMock.XUnit;
using Sudoku.Api.Controllers;
using System.Net;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.API;

public class GamesControllerTests : BaseTestByType<GamesController>
{
    private readonly string _playerAlias;
    private readonly string _gameId;
    private readonly GamesController _sut;
    private readonly Mock<IGameService> _mockGameService;
    private readonly IEnumerable<GameStateMemory> _games;

    public GamesControllerTests()
    {
        _games = Container.CreateMany<GameStateMemory>();
        _playerAlias = Container.Create<string>();
        _gameId = Container.Create<string>();
        _mockGameService = Container.ResolveMock<IGameService>();
        _sut = ResolveSut();

        _mockGameService.SetUpReturnedGame(_games.First());
        _mockGameService.SetUpReturnedGames(_games);
    }

    [Fact]
    public async Task CreateAsync_WhenValidAliasAndDifficulty_ReturnsStatusCode201()
    {
        // Act
        var response = await _sut.CreateAsync(_playerAlias, GameDifficulty.Medium);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenAliasIsNullOrWhitespace_ReturnsStatusCode400(string alias)
    {
        // Act
        var response = await _sut.CreateAsync(alias, GameDifficulty.Medium);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_WhenValidAliasAndDifficulty_CallsGameServiceWithCorrectParameters()
    {
        // Act
        await _sut.CreateAsync(_playerAlias, GameDifficulty.Medium);

        // Assert
        _mockGameService.VerifyCreateGameAsyncCalled(_playerAlias, GameDifficulty.Medium, Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteAsync_WhenAliasIsNullOrWhitespace_ReturnsStatusCode400(string alias)
    {
        // Act
        var response = await _sut.DeleteAsync(alias, _gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteAsync_WhenGameIdIsNullOrWhitespace_ReturnsStatusCode400(string gameId)
    {
        // Act
        var response = await _sut.DeleteAsync(_playerAlias, gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAsync_WhenValidAlias_CallsGameService()
    {
        // Act
        await _sut.DeleteAsync(_playerAlias, _gameId);

        // Assert
        _mockGameService.VerifyDeleteGameAsyncCalled(_playerAlias, _gameId, Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenValidAlias_ReturnsStatusCode204()
    {
        // Act
        var response = await _sut.DeleteAsync(_playerAlias, _gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAsync_WhenNoGameId_CallsGameServiceToRetrievePlayerGames()
    {
        // Act
        await _sut.GetAllAsync(_playerAlias);

        // Assert
        _mockGameService.VerifyGetGamesForAliasCalled(_playerAlias, Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenGivenGameId_CallsGameServiceToRetrieveSingleGame()
    {
        // Act
        await _sut.GetAsync(_playerAlias, _gameId);

        // Assert
        _mockGameService.VerifyGetGameForAliasCalled(_playerAlias, _gameId, Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAsync_WhenAliasIsNullOrWhitespace_ReturnsStatusCode400(string alias)
    {
        // Act
        var response = await _sut.GetAllAsync(alias);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAsync_WhenAliasIsNullOrWhitespaceWithGameId_ReturnsStatusCode400(string alias)
    {
        // Act
        var response = await _sut.GetAsync(alias, _gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAsync_WhenInvalidGameIdValidAlias_ReturnsStatusCode400(string gameId)
    {
        // Act
        var response = await _sut.GetAsync(_playerAlias, gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAsync_WhenPlayerHasNoGames_ReturnsEmptyList()
    {
        // Arrange
        _mockGameService.SetUpReturnedGames([]);

        // Act
        var response = await _sut.GetAllAsync(_playerAlias);

        // Assert
        response.AssertResponseReturnEquals([]);
    }

    [Fact]
    public async Task GetAsync_WhenNoMatchForAliasGameId_ReturnsStatusCode404()
    {
        // Arrange
        _mockGameService.SetUpReturnedGame(null);

        // Act
        var response = await _sut.GetAsync(_playerAlias, _gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAsync_WhenValidAlias_CallsGameService()
    {
        // Act
        await _sut.GetAllAsync(_playerAlias);

        // Assert
        _mockGameService.VerifyGetGamesForAliasCalled(_playerAlias, Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenValidAlias_ReturnsListOfGames()
    {
        // Act
        var response = await _sut.GetAllAsync(_playerAlias);

        // Assert
        response.AssertResponseReturnEquals(_games);
    }

    [Fact]
    public async Task GetAsync_WhenValidAliasValidGameId_ReturnsSingleGame()
    {
        // Act
        var response = await _sut.GetAsync(_playerAlias, _gameId);

        // Assert
        response.AssertResponseReturnEquals(_games.First());
    }

    [Fact]
    public async Task GetAsync_WhenValidAlias_ReturnsStatusCode200()
    {
        // Act
        var response = await _sut.GetAllAsync(_playerAlias);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAsync_WhenValidAliasValidGameId_ReturnsStatusCode200()
    {
        // Act
        var response = await _sut.GetAsync(_playerAlias, _gameId);

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.OK);
    }
}