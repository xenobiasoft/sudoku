using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace UnitTests.API;

public class GameStatusControllerTests : BaseGameControllerTests<GameStatusController>
{
    #region PauseGameAsync Tests

    [Fact]
    public async Task PauseGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<PauseGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.PauseGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task PauseGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.PauseGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PauseGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.PauseGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PauseGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.PauseGameAsync("TestPlayer", Guid.NewGuid().ToString());

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task PauseGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.PauseGameAsync("TestPlayer", gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PauseGameAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to pause game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<PauseGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.PauseGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region ResumeGameAsync Tests

    [Fact]
    public async Task ResumeGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ResumeGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.ResumeGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ResumeGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.ResumeGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResumeGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.ResumeGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResumeGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ResumeGameAsync("TestPlayer", Guid.NewGuid().ToString());

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ResumeGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.ResumeGameAsync("TestPlayer", gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ResumeGameAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to resume game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ResumeGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.ResumeGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region AbandonGameAsync Tests

    [Fact]
    public async Task AbandonGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<AbandonGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.AbandonGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AbandonGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.AbandonGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AbandonGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.AbandonGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AbandonGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.AbandonGameAsync("TestPlayer", Guid.NewGuid().ToString());

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task AbandonGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.AbandonGameAsync("TestPlayer", gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AbandonGameAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to abandon game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<AbandonGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.AbandonGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region CompleteGameAsync Tests

    [Fact]
    public async Task CompleteGameAsync_WithValidParameters_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<CompleteGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await Sut.CompleteGameAsync(playerAlias, gameId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task CompleteGameAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.CompleteGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CompleteGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.CompleteGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CompleteGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.CompleteGameAsync("TestPlayer", Guid.NewGuid().ToString());

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task CompleteGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.CompleteGameAsync("TestPlayer", gameId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CompleteGameAsync_WhenCommandFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto(playerAlias, "Medium", gameId);
        var errorMessage = "Failed to complete game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<CompleteGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await Sut.CompleteGameAsync(playerAlias, gameId);

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ValidateGameQuery>(), It.IsAny<CancellationToken>()))
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
        // Act
        var result = await Sut.ValidateGameAsync(string.Empty, Guid.NewGuid().ToString());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateGameAsync_WithEmptyGameId_ReturnsBadRequest()
    {
        // Act
        var result = await Sut.ValidateGameAsync("TestPlayer", string.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateGameAsync_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var errorMessage = "Failed to get game";

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ValidateGameAsync("TestPlayer", Guid.NewGuid().ToString());

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ValidateGameAsync_WhenGameBelongsToAnotherPlayer_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGameDto("OtherPlayer", "Medium", gameId);

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        // Act
        var result = await Sut.ValidateGameAsync("TestPlayer", gameId);

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

        MockMediator
            .Setup(x => x.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<GameDto>.Success(game));

        MockMediator
            .Setup(x => x.Send(It.IsAny<ValidateGameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ValidationResultDto>.Failure(errorMessage));

        // Act
        var result = await Sut.ValidateGameAsync(playerAlias, gameId);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    #endregion
}
