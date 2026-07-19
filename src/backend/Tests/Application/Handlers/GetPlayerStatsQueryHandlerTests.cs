using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class GetPlayerStatsQueryHandlerTests
    : MoqBaseTestByAbstraction<GetPlayerStatsQueryHandler, IQueryHandler<GetPlayerStatsQuery, PlayerStatsDto>>
{
    private readonly Mock<IGameCompletionRepository> _mockCompletionRepository;
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly string _profileId = Guid.NewGuid().ToString();

    public GetPlayerStatsQueryHandlerTests()
    {
        _mockCompletionRepository = Container.ResolveMock<IGameCompletionRepository>().AsMoq();
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithWinsAndActiveGames_CalculatesGamesPlayedAsWinsPlusActiveGames()
    {
        // Arrange — 3 wins + 2 games still in progress
        _mockCompletionRepository.SetupGetByProfileId([
            Completion("Easy", TimeSpan.FromMinutes(5)),
            Completion("Easy", TimeSpan.FromMinutes(7)),
            Completion("Hard", TimeSpan.FromMinutes(20))
        ]);
        _mockGameRepository.SetupGetByProfileId([
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy),
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Medium)
        ]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(new { GamesPlayed = 5, GamesWon = 3 });
    }

    [Fact]
    public async Task Handle_WithWinsAndActiveGames_CalculatesWinRateAsWinsOverGamesPlayed()
    {
        // Arrange — 1 win, 3 active → 1/4
        _mockCompletionRepository.SetupGetByProfileId([Completion("Easy", TimeSpan.FromMinutes(5))]);
        _mockGameRepository.SetupGetByProfileId([
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy),
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy),
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy)
        ]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.WinRate.Should().Be(0.25);
    }

    [Fact]
    public async Task Handle_WithCompletedGameDocumentStillPresent_ExcludesItFromGamesPlayed()
    {
        // Arrange — the client deletes solved games, but a transiently-present completed
        // document must not be counted alongside its own completion record.
        _mockCompletionRepository.SetupGetByProfileId([Completion("Easy", TimeSpan.FromMinutes(5))]);
        _mockGameRepository.SetupGetByProfileId([GameFactory.CreateCompletedGame()]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(new { GamesPlayed = 1, GamesWon = 1, WinRate = 1.0 });
    }

    [Fact]
    public async Task Handle_WithMultipleWinsAtOneDifficulty_CalculatesAverageAndBestSolveTime()
    {
        // Arrange
        _mockCompletionRepository.SetupGetByProfileId([
            Completion("Medium", TimeSpan.FromMinutes(10)),
            Completion("Medium", TimeSpan.FromMinutes(20))
        ]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Medium" && d.Size == 9)
            .Should().BeEquivalentTo(new
            {
                GamesPlayed = 2,
                GamesWon = 2,
                AverageSolveTime = TimeSpan.FromMinutes(15),
                BestSolveTime = TimeSpan.FromMinutes(10)
            });
    }

    [Fact]
    public async Task Handle_WhenAverageLandsOnSubSecondPrecision_TruncatesItToWholeSeconds()
    {
        // Arrange — averaging 1s and 2s gives 1.5s, which System.Text.Json would emit as
        // "00:00:01.5000000", breaking the "HH:MM:SS" contract the clients parse.
        _mockCompletionRepository.SetupGetByProfileId([
            Completion("Hard", TimeSpan.FromSeconds(1)),
            Completion("Hard", TimeSpan.FromSeconds(2))
        ]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Hard" && d.Size == 9)
            .AverageSolveTime.Should().Be(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Handle_WithNoWinsAtADifficulty_ReturnsNullSolveTimesForThatDifficulty()
    {
        // Arrange — an in-progress Expert game, but no Expert win
        _mockCompletionRepository.SetupGetByProfileId([Completion("Easy", TimeSpan.FromMinutes(5))]);
        _mockGameRepository.SetupGetByProfileId([GameFactory.CreateGameWithDifficulty(GameDifficulty.Expert)]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Expert" && d.Size == 9)
            .Should().BeEquivalentTo(new
            {
                GamesPlayed = 1,
                GamesWon = 0,
                AverageSolveTime = (TimeSpan?)null,
                BestSolveTime = (TimeSpan?)null
            });
    }

    [Fact]
    public async Task Handle_WithAnyPlayer_ReturnsAllEightSizeDifficultyRowsInFixedOrder()
    {
        // Arrange — 9x9 x 4 difficulties, then 16x16 x 4 difficulties
        _mockCompletionRepository.SetupGetByProfileId([]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Select(d => (d.Size, d.Difficulty))
            .Should().Equal(
                (9, "Easy"), (9, "Medium"), (9, "Hard"), (9, "Expert"),
                (16, "Easy"), (16, "Medium"), (16, "Hard"), (16, "Expert"));
    }

    [Fact]
    public async Task Handle_With16x16Completion_CountsItUnder16x16RowOnly()
    {
        // Arrange
        _mockCompletionRepository.SetupGetByProfileId([Completion("Medium", TimeSpan.FromMinutes(5), gridSize: 16)]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Medium" && d.Size == 16)
            .Should().BeEquivalentTo(new { GamesPlayed = 1, GamesWon = 1 });
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Medium" && d.Size == 9)
            .Should().BeEquivalentTo(new { GamesPlayed = 0, GamesWon = 0 });
    }

    [Fact]
    public async Task Handle_With16x16ActiveGame_CountsItUnder16x16RowOnly()
    {
        // Arrange
        _mockCompletionRepository.SetupGetByProfileId([]);
        _mockGameRepository.SetupGetByProfileId([
            GameFactory.CreateGameWithDifficulty(GameDifficulty.Hard, BoardSize.Sixteen)
        ]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Hard" && d.Size == 16)
            .Should().BeEquivalentTo(new { GamesPlayed = 1, GamesWon = 0 });
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Hard" && d.Size == 9)
            .Should().BeEquivalentTo(new { GamesPlayed = 0, GamesWon = 0 });
    }

    [Fact]
    public async Task Handle_WithLegacyCompletionMissingGridSize_CountsItAs9x9()
    {
        // Arrange — a legacy GameCompletionDocument with no gridSize deserializes as GridSize == 9
        _mockCompletionRepository.SetupGetByProfileId([Completion("Easy", TimeSpan.FromMinutes(5), gridSize: 9)]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Easy" && d.Size == 9)
            .Should().BeEquivalentTo(new { GamesPlayed = 1, GamesWon = 1 });
        result.Value.ByDifficulty.Single(d => d.Difficulty == "Easy" && d.Size == 16)
            .Should().BeEquivalentTo(new { GamesPlayed = 0, GamesWon = 0 });
    }

    [Fact]
    public async Task Handle_WithNoGamesAtAll_ReturnsZeroedStatsWithoutDividingByZero()
    {
        // Arrange
        _mockCompletionRepository.SetupGetByProfileId([]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(new { GamesPlayed = 0, GamesWon = 0, WinRate = 0.0 });
    }

    [Fact]
    public async Task Handle_WithNoGamesAtAll_ReturnsSuccess()
    {
        // Arrange — an empty player is a valid result, not an error
        _mockCompletionRepository.SetupGetByProfileId([]);
        _mockGameRepository.SetupGetByProfileId([]);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidProfileId_ReturnsFailure()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery("not-a-guid"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenCompletionRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        const string exceptionMessage = "Cosmos is down";
        _mockCompletionRepository.SetupGetByProfileIdThrows(new Exception(exceptionMessage));
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }

    [Fact]
    public async Task Handle_WhenGameRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        const string exceptionMessage = "Cosmos is down";
        _mockCompletionRepository.SetupGetByProfileId([]);
        _mockGameRepository.SetupGetByProfileIdThrows(new Exception(exceptionMessage));
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new GetPlayerStatsQuery(_profileId), CancellationToken.None);

        // Assert
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }

    private GameCompletion Completion(string difficulty, TimeSpan playDuration, int gridSize = 9) =>
        new(Guid.NewGuid().ToString(), _profileId, difficulty, playDuration, DateTime.UtcNow, gridSize);
}
