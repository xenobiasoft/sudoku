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

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<GetGameQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<GetGameQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<GetPlayerGamesByStatusQuery>(), default))
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

        _mockMediator.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), default))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.MakeMoveAsync(gameId, row, column, value);

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

        _mockMediator.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), default))
            .ReturnsAsync(Result.Failure(errorMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.MakeMoveAsync(gameId, row, column, value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task StartGameAsync_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();

        _mockMediator.Setup(m => m.Send(It.IsAny<StartGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.StartGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task PauseGameAsync_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();

        _mockMediator.Setup(m => m.Send(It.IsAny<PauseGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.PauseGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ResumeGameAsync_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();

        _mockMediator.Setup(m => m.Send(It.IsAny<ResumeGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.ResumeGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AbandonGameAsync_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();

        _mockMediator.Setup(m => m.Send(It.IsAny<AbandonGameCommand>(), default))
            .ReturnsAsync(Result.Success());

        var sut = ResolveSut();

        // Act
        var result = await sut.AbandonGameAsync(gameId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}