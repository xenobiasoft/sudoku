using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
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
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());

        // Act
        var result = await Sut.MakeMoveAsync(string.Empty, Guid.NewGuid().ToString(), move);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task MakeMoveAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var move = new MoveRequest(1, 1, 5, Container.Create<TimeSpan>());

        // Act
        var result = await Sut.MakeMoveAsync("TestPlayer", string.Empty, move);

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ResetGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.ResetGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.ResetGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.ResetGameAsync("TestPlayer", string.Empty);

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ResetGameCommand>(), It.IsAny<CancellationToken>()))
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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<UndoLastMoveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.UndoMoveAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UndoMoveAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.UndoMoveAsync("TestPlayer", string.Empty);

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<UndoLastMoveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.UndoMoveAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion
}
