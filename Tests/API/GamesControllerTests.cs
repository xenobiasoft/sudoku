using DepenMock.XUnit;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.API;

public class GamesControllerTests : BaseTestByType<GamesController>
{
    private readonly string _playerAlias;
    private readonly GamesController _sut;
    private readonly Mock<IGameService> _mockGameService;
    private readonly IEnumerable<GameStateMemory> _games;

    public GamesControllerTests()
    {
        _games = Container.CreateMany<GameStateMemory>();
        _playerAlias = Container.Create<string>();
        _mockGameService = Container.ResolveMock<IGameService>();
        _sut = ResolveSut();

        _mockGameService.SetUpReturnedGames(_games);
    }

    [Fact]
    public async Task Get_WhenPlayerHasNoGames_ReturnsEmptyList()
    {
        // Arrange
        _mockGameService.SetUpReturnedGames([]);

        // Act
        var response = await _sut.GetAsync(_playerAlias);

        // Assert
        response.AssertResponseReturnEquals([]);
    }

    [Fact]
    public async Task Get_WhenValidAlias_CallsGameService()
    {
        // Act
        await _sut.GetAsync(_playerAlias);

        // Assert
        _mockGameService.VerifyGetGamesForAliasCalled(_playerAlias, Times.Once);
    }

    [Fact]
    public async Task Get_WhenValidAlias_ReturnsListOfGames()
    {
        // Act
        var response = await _sut.GetAsync(_playerAlias);

        // Assert
        response.AssertResponseReturnEquals(_games);
    }

    // 2.3 GET /api/players/{alias}/games returns a list of games with the correct player alias.
    // 2.4 GET /api/players/{alias}/games returns 200 OK with a list of games.
    // 2.5 GET /api/players/{alias}/games calls GameService to retrieve games for the player alias.
    // 3. DELETE /api/players/{alias}/games returns 204 No Content after deleting all games for the player alias.
    // 3.1 DELETE /api/players/{alias}/games returns 404 Not Found if the player alias does not exist.
    // 3.2 DELETE /api/players/{alias}/games deletes all games for the player alias.
}