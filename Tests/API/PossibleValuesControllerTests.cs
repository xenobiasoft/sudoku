using DepenMock.XUnit;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace UnitTests.API;

public class PossibleValuesControllerTests : BaseTestByType<PossibleValuesController>
{
    private readonly Mock<IGameApplicationService> _mockGameService;
    private readonly PossibleValuesController _sut;

    public PossibleValuesControllerTests()
    {
        _mockGameService = Container.ResolveMock<IGameApplicationService>();
        _sut = ResolveSut();
    }

    #region AddPossibleValueAsync Tests

    [Fact]
    public async Task AddPossibleValueAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, gameId);

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        _mockGameService
            .Setup(x => x.AddPossibleValueAsync(gameId, request.Row, request.Column, request.Value))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.AddPossibleValueAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await _sut.AddPossibleValueAsync(emptyAlias, gameId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await _sut.AddPossibleValueAsync(playerAlias, emptyGameId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var errorMessage = "Failed to get game";

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.AddPossibleValueAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(differentPlayerAlias, gameId);

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.AddPossibleValueAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenAddPossibleValueFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, gameId);
        var errorMessage = "Failed to add possible value";

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        _mockGameService
            .Setup(x => x.AddPossibleValueAsync(gameId, request.Row, request.Column, request.Value))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.AddPossibleValueAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region ClearPossibleValuesAsync Tests

    [Fact]
    public async Task ClearPossibleValuesAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);
        var game = CreateTestGameDto(playerAlias, gameId);

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        _mockGameService
            .Setup(x => x.ClearPossibleValuesAsync(gameId, request.Row, request.Column))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var emptyAlias = string.Empty;
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);

        // Act
        var result = await _sut.ClearPossibleValuesAsync(emptyAlias, gameId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var emptyGameId = string.Empty;
        var request = new CellRequest(1, 1);

        // Act
        var result = await _sut.ClearPossibleValuesAsync(playerAlias, emptyGameId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenGetGameReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);
        var errorMessage = "Failed to get game";

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await _sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var differentPlayerAlias = "OtherPlayer";
        var request = new CellRequest(1, 1);
        var game = CreateTestGameDto(differentPlayerAlias, gameId);

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await _sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenClearPossibleValuesFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);
        var game = CreateTestGameDto(playerAlias, gameId);
        var errorMessage = "Failed to clear possible values";

        _mockGameService
            .Setup(x => x.GetGameAsync(gameId))
            .ReturnsAsync(Result<GameDto>.Success(game));

        _mockGameService
            .Setup(x => x.ClearPossibleValuesAsync(gameId, request.Row, request.Column))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region Helper Methods

    private static GameDto CreateTestGameDto(string playerAlias, string gameId)
    {
        var statistics = new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0);
        
        return new GameDto(
            gameId,
            playerAlias,
            "Medium",
            "InProgress",
            statistics,
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