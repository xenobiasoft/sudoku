using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

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
        
        MockGameService
            .Setup(x => x.GetPlayerGamesAsync(playerAlias))
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
        // Arrange
        var emptyAlias = string.Empty;

        // Act
        var result = await Sut.GetAllGamesAsync(emptyAlias);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to get games";
        
        MockGameService
            .Setup(x => x.GetPlayerGamesAsync(playerAlias))
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
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
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
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();

        // Act
        var result = await Sut.GetGameAsync(emptyAlias, gameId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;

        // Act
        var result = await Sut.GetGameAsync(playerAlias, emptyGameId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetGameAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Failed to get game";
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.GetGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.GetGameAsync(playerAlias, gameId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region CreateGameAsync Tests

    [Fact]
    public async Task CreateGameAsync_WithValidParameters_ReturnsCreatedAtAction()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, difficulty, gameId);
        
        MockGameService
            .Setup(x => x.CreateGameAsync(playerAlias, difficulty))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedResult>().Subject;
        createdAtActionResult.Location.Should().Be($"/api/players/{playerAlias}/games/{gameId}");
        var returnedGame = createdAtActionResult.Value.Should().BeAssignableTo<GameDto>().Subject;
        returnedGame.Should().BeEquivalentTo(game);
    }

    [Fact]
    public async Task CreateGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var difficulty = "Medium";

        // Act
        var result = await Sut.CreateGameAsync(emptyAlias, difficulty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateGameAsync_WithEmptyDifficulty_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyDifficulty = string.Empty;

        // Act
        var result = await Sut.CreateGameAsync(playerAlias, emptyDifficulty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateGameAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var errorMessage = "Failed to create game";
        
        MockGameService
            .Setup(x => x.CreateGameAsync(playerAlias, difficulty))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
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
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        MockGameService
            .Setup(x => x.DeleteGameAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();

        // Act
        var result = await Sut.DeleteGameAsync(emptyAlias, gameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, emptyGameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Failed to get game";
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.DeleteGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DeleteGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
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
        
        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        MockGameService
            .Setup(x => x.DeleteGameAsync(gameId))
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
        
        MockGameService
            .Setup(x => x.DeletePlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.DeleteAllGamesAsync(playerAlias);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;

        // Act
        var result = await Sut.DeleteAllGamesAsync(emptyAlias);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to delete games";
        
        MockGameService
            .Setup(x => x.DeletePlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.DeleteAllGamesAsync(playerAlias);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region Helper Methods

    private static GameDto CreateTestGameDto(string playerAlias, string difficulty, string? gameId = null)
    {
        return new GameDto(
            gameId ?? Guid.NewGuid().ToString(),
            playerAlias,
            difficulty,
            "NotStarted",
            new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0),
            DateTime.UtcNow,
            null,
            null,
            null,
            new List<CellDto>(),
            new List<MoveHistoryDto>()
        );
    }

    #endregion
}