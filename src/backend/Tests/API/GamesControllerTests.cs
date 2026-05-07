using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace UnitTests.API;

public class GamesControllerTests : BaseGameControllerTests<GamesController>
{
    #region GetAllGamesAsync Tests

    [Fact]
    public async Task GetAllGamesAsync_WithValidAlias_ReturnsOkWithGames()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var games = new List<GameDto>
        {
            CreateTestGameDto(playerAlias, "Medium")
        };

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetPlayerGamesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<GameDto>>.Success(games));

        // Act
        var result = await Sut.GetAllGamesAsync(playerAlias);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGames = okResult.Value.Should().BeAssignableTo<List<GameDto>>().Subject;
        returnedGames.Should().BeEquivalentTo(games);
    }

    [Fact]
    public async Task GetAllGamesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.GetAllGamesAsync(string.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to get games";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetPlayerGamesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<GameDto>>.Failure(errorMessage));

        // Act
        var result = await Sut.GetAllGamesAsync(playerAlias);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region GetGameAsync Tests

    [Fact]
    public async Task GetGameAsync_WithValidParameters_ReturnsOkWithGame()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.GetGameAsync(playerAlias, gameId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGame = okResult.Value.Should().BeAssignableTo<GameDto>().Subject;
        returnedGame.Should().BeEquivalentTo(game);
    }

    [Fact]
    public async Task GetGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.GetGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.GetGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Game not found with ID: " + gameId;

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.GetGameAsync(playerAlias, gameId);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.GetGameAsync(playerAlias, gameId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region CreateGameAsync Tests

    [Fact]
    public async Task CreateGameAsync_WithValidParameters_ReturnsCreated()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var gameId = Guid.NewGuid().ToString();

        MockMediator
            .Setup(x => x.Send(It.IsAny<CreateGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success(gameId));

        // Act
        var result = await Sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be($"/api/players/{playerAlias}/games/{gameId}");
        createdResult.Value.Should().BeNull();
    }

    [Fact]
    public async Task CreateGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.CreateGameAsync(string.Empty, "Medium");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateGameAsync_WithEmptyDifficulty_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.CreateGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateGameAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var errorMessage = "Failed to create game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<CreateGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure(errorMessage));

        // Act
        var result = await Sut.CreateGameAsync("TestPlayer", "Medium");

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region DeleteGameAsync Tests

    [Fact]
    public async Task DeleteGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<DeleteGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.DeleteGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.DeleteGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Game not found with ID: " + gameId;

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WhenDeleteGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to delete game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<DeleteGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region DeleteAllGamesAsync Tests

    [Fact]
    public async Task DeleteAllGamesAsync_WithValidAlias_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";

        MockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.DeleteAllGamesAsync(playerAlias);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.DeleteAllGamesAsync(string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var errorMessage = "Failed to delete games";

        MockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.DeleteAllGamesAsync("TestPlayer");

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion
}
