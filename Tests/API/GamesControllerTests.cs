using DepenMock.XUnit;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace UnitTests.API;

public class GamesControllerTests : BaseTestByType<GamesController>
{
    private readonly Mock<IGameApplicationService> _mockGameService;
    private readonly GamesController _sut;

    public GamesControllerTests()
    {
        _mockGameService = Container.ResolveMock<IGameApplicationService>();
        _sut = ResolveSut();
    }

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
        
        _mockGameService
            .Setup(x => x.GetPlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result<List<GameDto>>.Success(games));

        // Act
        var result = await _sut.GetAllGamesAsync(playerAlias);

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
        var result = await _sut.GetAllGamesAsync(emptyAlias);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to get games";
        
        _mockGameService
            .Setup(x => x.GetPlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result<List<GameDto>>.Failure(errorMessage));

        // Act
        var result = await _sut.GetAllGamesAsync(playerAlias);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.GetGameAsync(playerAlias, gameId);

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
        var result = await _sut.GetGameAsync(emptyAlias, gameId);

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
        var result = await _sut.GetGameAsync(playerAlias, emptyGameId);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.GetGameAsync(playerAlias, gameId);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.GetGameAsync(playerAlias, gameId);

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
        
        _mockGameService
            .Setup(x => x.CreateGameAsync(playerAlias, difficulty))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(GamesController.GetGameAsync));
        createdAtActionResult.RouteValues.Should().ContainKey("alias").WhoseValue.Should().Be(playerAlias);
        createdAtActionResult.RouteValues.Should().ContainKey("gameId").WhoseValue.Should().Be(gameId);
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
        var result = await _sut.CreateGameAsync(emptyAlias, difficulty);

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
        var result = await _sut.CreateGameAsync(playerAlias, emptyDifficulty);

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
        
        _mockGameService
            .Setup(x => x.CreateGameAsync(playerAlias, difficulty))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region UpdateGameAsync Tests

    [Fact]
    public async Task UpdateGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.MakeMoveAsync(gameId, move.Row, move.Column, move.Value))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.UpdateGameAsync(playerAlias, gameId, move);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5);

        // Act
        var result = await _sut.UpdateGameAsync(emptyAlias, gameId, move);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;
        var move = new MoveRequest(1, 1, 5);

        // Act
        var result = await _sut.UpdateGameAsync(playerAlias, emptyGameId, move);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateGameAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5);
        var errorMessage = "Failed to get game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.UpdateGameAsync(playerAlias, gameId, move);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5);
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.UpdateGameAsync(playerAlias, gameId, move);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateGameAsync_WhenMakeMoveReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to make move";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.MakeMoveAsync(gameId, move.Row, move.Column, move.Value))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.UpdateGameAsync(playerAlias, gameId, move);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.DeleteGameAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.DeleteGameAsync(playerAlias, gameId);

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
        var result = await _sut.DeleteGameAsync(emptyAlias, gameId);

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
        var result = await _sut.DeleteGameAsync(playerAlias, emptyGameId);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.DeleteGameAsync(playerAlias, gameId);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.DeleteGameAsync(playerAlias, gameId);

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
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.DeleteGameAsync(gameId))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.DeleteGameAsync(playerAlias, gameId);

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
        
        _mockGameService
            .Setup(x => x.DeletePlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.DeleteAllGamesAsync(playerAlias);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;

        // Act
        var result = await _sut.DeleteAllGamesAsync(emptyAlias);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteAllGamesAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to delete games";
        
        _mockGameService
            .Setup(x => x.DeletePlayerGamesAsync(playerAlias))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.DeleteAllGamesAsync(playerAlias);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region UndoMoveAsync Tests

    [Fact]
    public async Task UndoMoveAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.UndoLastMoveAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();

        // Act
        var result = await _sut.UndoMoveAsync(emptyAlias, gameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;

        // Act
        var result = await _sut.UndoMoveAsync(playerAlias, emptyGameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Failed to get game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UndoMoveAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WhenUndoLastMoveReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to undo move";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.UndoLastMoveAsync(gameId))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region ResetGameAsync Tests

    [Fact]
    public async Task ResetGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.ResetGameAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.ResetGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();

        // Act
        var result = await _sut.ResetGameAsync(emptyAlias, gameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;

        // Act
        var result = await _sut.ResetGameAsync(playerAlias, emptyGameId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Failed to get game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.ResetGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ResetGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.ResetGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WhenResetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to reset game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.ResetGameAsync(gameId))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.ResetGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region ValidateGameAsync Tests

    [Fact]
    public async Task ValidateGameAsync_WithValidParameters_ReturnsOkWithValidationResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var validationResult = new ValidationResultDto(true, new List<string>());
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.ValidateGameAsync(gameId))
            .ReturnsAsync(Result<ValidationResultDto>.Success(validationResult));

        // Act
        var result = await _sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValidationResult = okResult.Value.Should().BeAssignableTo<ValidationResultDto>().Subject;
        returnedValidationResult.Should().BeEquivalentTo(validationResult);
    }

    [Fact]
    public async Task ValidateGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();

        // Act
        var result = await _sut.ValidateGameAsync(emptyAlias, gameId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;

        // Act
        var result = await _sut.ValidateGameAsync(playerAlias, emptyGameId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateGameAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Failed to get game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ValidateGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ValidateGameAsync_WhenValidateGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to validate game";
        
        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));
        
        _mockGameService
            .Setup(x => x.ValidateGameAsync(gameId))
            .ReturnsAsync(Result<ValidationResultDto>.Failure(errorMessage));

        // Act
        var result = await _sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
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
            new List<CellDto>()
        );
    }

    #endregion
}