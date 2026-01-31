using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace UnitTests.API;

public class GameStatusControllerTests : BaseGameControllerTests<GameStatusController>
{
    #region UpdateGameStatusAsync Tests

    [Fact]
    public async Task UpdateGameStatusAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var gameStatus = "InProgress";
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.UpdateGameStatusAsync(gameId, gameStatus))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.UpdateGameStatusAsync(playerAlias, gameId, gameStatus);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();
        var gameStatus = "Completed";

        // Act
        var result = await Sut.UpdateGameStatusAsync(emptyAlias, gameId, gameStatus);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;
        var gameStatus = "Completed";

        // Act
        var result = await Sut.UpdateGameStatusAsync(playerAlias, emptyGameId, gameStatus);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var gameStatus = "Completed";
        var errorMessage = "Failed to get game";

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.UpdateGameStatusAsync(playerAlias, gameId, gameStatus);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var game = CreateTestGameDto(differentPlayerAlias, "Medium", gameId);
        var gameStatus = "Completed";

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.UpdateGameStatusAsync(playerAlias, gameId, gameStatus);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WhenUpdateGameStatusReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var gameStatus = "Completed";
        var errorMessage = "Failed to update game status";

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.UpdateGameStatusAsync(gameId, gameStatus))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.UpdateGameStatusAsync(playerAlias, gameId, gameStatus);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.ValidateGameAsync(gameId))
            .ReturnsAsync(Result<ValidationResultDto>.Success(validationResult));

        // Act
        var result = await Sut.ValidateGameAsync(playerAlias, gameId);

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
        var result = await Sut.ValidateGameAsync(emptyAlias, gameId);

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
        var result = await Sut.ValidateGameAsync(playerAlias, emptyGameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ValidateGameAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.ValidateGameAsync(playerAlias, gameId);

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

        MockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockGameService
            .Setup(x => x.ValidateGameAsync(gameId))
            .ReturnsAsync(Result<ValidationResultDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion
}