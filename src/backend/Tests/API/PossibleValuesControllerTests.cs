using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace UnitTests.API;

public class PossibleValuesControllerTests : BaseGameControllerTests<PossibleValuesController>
{
    #region AddPossibleValueAsync Tests

    [Fact]
    public async Task AddPossibleValueAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<AddPossibleValueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.AddPossibleValueAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await Sut.AddPossibleValueAsync(string.Empty, Guid.NewGuid().ToString(), request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await Sut.AddPossibleValueAsync("TestPlayer", string.Empty, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.AddPossibleValueAsync("TestPlayer", Guid.NewGuid().ToString(), request);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.AddPossibleValueAsync("TestPlayer", gameId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddPossibleValueAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to add possible value";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<AddPossibleValueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.AddPossibleValueAsync(playerAlias, gameId, request);

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
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ClearPossibleValuesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var request = new CellRequest(1, 1);

        // Act
        var result = await Sut.ClearPossibleValuesAsync(string.Empty, Guid.NewGuid().ToString(), request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CellRequest(1, 1);

        // Act
        var result = await Sut.ClearPossibleValuesAsync("TestPlayer", string.Empty, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new CellRequest(1, 1);
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ClearPossibleValuesAsync("TestPlayer", Guid.NewGuid().ToString(), request);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.ClearPossibleValuesAsync("TestPlayer", gameId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ClearPossibleValuesAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new CellRequest(1, 1);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to clear possible values";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ClearPossibleValuesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.ClearPossibleValuesAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region RemovePossibleValueAsync Tests

    [Fact]
    public async Task RemovePossibleValueAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<RemovePossibleValueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.RemovePossibleValueAsync(playerAlias, gameId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemovePossibleValueAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await Sut.RemovePossibleValueAsync(string.Empty, Guid.NewGuid().ToString(), request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemovePossibleValueAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);

        // Act
        var result = await Sut.RemovePossibleValueAsync("TestPlayer", string.Empty, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemovePossibleValueAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new PossibleValueRequest(1, 1, 5);
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.RemovePossibleValueAsync("TestPlayer", Guid.NewGuid().ToString(), request);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task RemovePossibleValueAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.RemovePossibleValueAsync("TestPlayer", gameId, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task RemovePossibleValueAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var request = new PossibleValueRequest(1, 1, 5);
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to remove possible value";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<RemovePossibleValueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.RemovePossibleValueAsync(playerAlias, gameId, request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion
}
