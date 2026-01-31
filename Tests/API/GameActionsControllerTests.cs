using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace UnitTests.API;

public class GameActionsControllerTests : BaseGameControllerTests<GameActionsController>
{
    #region MakeMoveAsync Tests

    [Fact]
    public async Task MakeMoveAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.MakeMoveAsync(gameId, move.Row, move.Column, move.Value, move.PlayDuration))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.MakeMoveAsync(playerAlias, gameId, move);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task MakeMoveAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());

        // Act
        var result = await Sut.MakeMoveAsync(emptyAlias, gameId, move);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task MakeMoveAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());

        // Act
        var result = await Sut.MakeMoveAsync(playerAlias, emptyGameId, move);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task MakeMoveAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());
        var errorMessage = "Failed to get game";

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.MakeMoveAsync(playerAlias, gameId, move);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task MakeMoveAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.MakeMoveAsync(playerAlias, gameId, move);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task MakeMoveAsync_WhenMakeMoveReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to make move";

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.MakeMoveAsync(gameId, move.Row, move.Column, move.Value, move.PlayDuration))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.MakeMoveAsync(playerAlias, gameId, move);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.ResetGameAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.ResetGameAsync(playerAlias, gameId);

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
        var result = await Sut.ResetGameAsync(emptyAlias, gameId);

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
        var result = await Sut.ResetGameAsync(playerAlias, emptyGameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ResetGameAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.ResetGameAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.ResetGameAsync(gameId))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.ResetGameAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.UndoLastMoveAsync(gameId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

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
        var result = await Sut.UndoMoveAsync(emptyAlias, gameId);

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
        var result = await Sut.UndoMoveAsync(playerAlias, emptyGameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.UndoLastMoveAsync(gameId))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

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