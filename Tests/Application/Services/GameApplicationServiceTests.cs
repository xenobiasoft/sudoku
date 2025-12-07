using DepenMock.XUnit;
using MediatR;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Application.Services;

namespace UnitTests.Application.Services;

public class GameApplicationServiceTests : BaseTestByAbstraction<GameApplicationService, IGameApplicationService>
{
    private readonly Mock<IMediator> _mockMediator;

    public GameApplicationServiceTests()
    {
        _mockMediator = Container.ResolveMock<IMediator>();
    }

    [Fact]
    public async Task CreateGameAsync_WithValidParameters_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var gameId = Guid.NewGuid().ToString();
        var gameDto = new GameDto(
            gameId,
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

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<List<GameDto>>.Success(new List<GameDto> { gameDto }));

        var sut = ResolveSut();

        // Act
        var result = await sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(gameId);
        result.Value.PlayerAlias.Should().Be(playerAlias);
        result.Value.Difficulty.Should().Be(difficulty);
    }

    [Fact]
    public async Task CreateGameAsync_WhenCommandFails_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var errorMessage = "Failed to create game";

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task CreateGameAsync_WhenGetPlayerGamesFails_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var errorMessage = "Failed to get player games";

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<List<GameDto>>.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task CreateGameAsync_WhenNoGamesReturned_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<List<GameDto>>.Success(new List<GameDto>()));

        var sut = ResolveSut();

        // Act
        var result = await sut.CreateGameAsync(playerAlias, difficulty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to retrieve created game");
    }

    [Fact]
    public async Task GetGameAsync_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var gameDto = new GameDto(
            gameId,
            "TestPlayer",
            "Medium",
            "InProgress",
            new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0),
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            null,
            new List<CellDto>()
        );

        _mockMediator.Setup(m => m.Send(It.IsAny<GetGameQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<GameDto>.Success(gameDto));

        var sut = ResolveSut();

        // Act
        var result = await sut.GetGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(gameId);
    }

    [Fact]
    public async Task GetGameAsync_WhenQueryFails_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var errorMessage = "Game not found";

        _mockMediator.Setup(m => m.Send(It.IsAny<GetGameQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<GameDto>.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.GetGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetPlayerGamesAsync_WithValidPlayerAlias_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var games = new List<GameDto>
        {
            new GameDto(
                Guid.NewGuid().ToString(),
                playerAlias,
                "Easy",
                "Completed",
                new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0),
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                new List<CellDto>()
            )
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<List<GameDto>>.Success(games));

        var sut = ResolveSut();

        // Act
        var result = await sut.GetPlayerGamesAsync(playerAlias);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().Be(1);
        result.Value.First().PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetPlayerGamesByStatusAsync_WithValidParameters_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var status = "InProgress";
        var games = new List<GameDto>
        {
            new GameDto(
                Guid.NewGuid().ToString(),
                playerAlias,
                "Hard",
                status,
                new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0),
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                null,
                new List<CellDto>()
            )
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesByStatusQuery>(), CancellationToken.None))
            .ReturnsAsync(Result<List<GameDto>>.Success(games));

        var sut = ResolveSut();

        // Act
        var result = await sut.GetPlayerGamesByStatusAsync(playerAlias, status);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Count.Should().Be(1);
        result.Value.First().Status.Should().Be(status);
    }

    [Fact]
    public async Task MakeMoveAsync_WithValidParameters_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;

        _mockMediator.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.MakeMoveAsync(gameId, row, column, value, TimeSpan.FromSeconds(30));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MakeMoveAsync_WhenCommandFails_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var errorMessage = "Invalid move";

        _mockMediator.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.MakeMoveAsync(gameId, row, column, value, TimeSpan.FromSeconds(30));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateGameStatusAsync_InProgress_UsesResumeCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "InProgress";

        _mockMediator.Setup(m => m.Send(It.IsAny<ResumeGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_Paused_UsesPauseCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "Paused";

        _mockMediator.Setup(m => m.Send(It.IsAny<PauseGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_Abandoned_UsesAbandonCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "Abandoned";

        _mockMediator.Setup(m => m.Send(It.IsAny<AbandonGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_Completed_UsesCompleteCommand()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "Completed";

        _mockMediator.Setup(m => m.Send(It.IsAny<CompleteGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateGameStatusAsync_NotStarted_ReturnsFailure()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "NotStarted";

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot manually set status to NotStarted");
    }

    [Fact]
    public async Task UpdateGameStatusAsync_InvalidStatus_ReturnsFailure()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "NotAStatus";

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Invalid game status: {status}");
    }

    [Fact]
    public async Task UpdateGameStatusAsync_WhenMediatorReturnsFailure_PropagatesFailure()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "InProgress";
        var errorMessage = "Failed to resume";

        _mockMediator.Setup(m => m.Send(It.IsAny<ResumeGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateGameStatusAsync_Completed_WhenMediatorFails_PropagatesFailure()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var status = "Completed";
        var errorMessage = "Failed to complete game";

        _mockMediator.Setup(m => m.Send(It.IsAny<CompleteGameCommand>(), CancellationToken.None))
            .ReturnsAsync(Result.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.UpdateGameStatusAsync(gameId, status);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }
}