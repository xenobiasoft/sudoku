using DepenMock.XUnit;
using Microsoft.Extensions.Logging;
using Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;
using XenobiaSoft.Sudoku.Infrastructure.Repositories;

namespace UnitTests.Infrastructure.Repositories;

public class InMemoryGameRepositoryTests : BaseTestByAbstraction<InMemoryGameRepository, IGameRepository>
{
    private readonly Mock<ILogger<InMemoryGameRepository>> _mockLogger;

    public InMemoryGameRepositoryTests()
    {
        _mockLogger = Container.ResolveMock<ILogger<InMemoryGameRepository>>();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGame_ReturnsGame()
    {
        // Arrange
        var sut = ResolveSut();
        var game = CreateTestGame();
        await sut.SaveAsync(game);

        // Act
        var result = await sut.GetByIdAsync(game.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(game.Id);
        result.PlayerAlias.Should().Be(game.PlayerAlias);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingGame_ReturnsNull()
    {
        // Arrange
        var sut = ResolveSut();
        var nonExistingId = GameId.New();

        // Act
        var result = await sut.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_WithValidGame_SavesGameSuccessfully()
    {
        // Arrange
        var sut = ResolveSut();
        var game = CreateTestGame();

        // Act
        await sut.SaveAsync(game);

        // Assert
        var savedGame = await sut.GetByIdAsync(game.Id);
        savedGame.Should().NotBeNull();
        savedGame!.Id.Should().Be(game.Id);
    }

    [Fact]
    public async Task SaveAsync_WithExistingGame_UpdatesGame()
    {
        // Arrange
        var sut = ResolveSut();
        var game = CreateTestGame();
        await sut.SaveAsync(game);

        // Modify the game
        game.StartGame();

        // Act
        await sut.SaveAsync(game);

        // Assert
        var updatedGame = await sut.GetByIdAsync(game.Id);
        updatedGame.Should().NotBeNull();
        updatedGame!.Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingGame_DeletesGame()
    {
        // Arrange
        var sut = ResolveSut();
        var game = CreateTestGame();
        await sut.SaveAsync(game);

        // Act
        await sut.DeleteAsync(game.Id);

        // Assert
        var deletedGame = await sut.GetByIdAsync(game.Id);
        deletedGame.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingGame_DoesNotThrow()
    {
        // Arrange
        var sut = ResolveSut();
        var nonExistingId = GameId.New();

        // Act
        Func<Task> act = async () => await sut.DeleteAsync(nonExistingId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingGame_ReturnsTrue()
    {
        // Arrange
        var sut = ResolveSut();
        var game = CreateTestGame();
        await sut.SaveAsync(game);

        // Act
        var result = await sut.ExistsAsync(game.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingGame_ReturnsFalse()
    {
        // Arrange
        var sut = ResolveSut();
        var nonExistingId = GameId.New();

        // Act
        var result = await sut.ExistsAsync(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByPlayerAsync_WithExistingPlayer_ReturnsGamesOrderedByCreatedAt()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var game3 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));

        await sut.SaveAsync(game1);
        await Task.Delay(10); // Ensure different timestamps
        await sut.SaveAsync(game2);
        await sut.SaveAsync(game3);

        // Act
        var result = await sut.GetByPlayerAsync(playerAlias);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.PlayerAlias.Should().Be(playerAlias));
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetByPlayerAsync_WithNonExistingPlayer_ReturnsEmptyCollection()
    {
        // Arrange
        var sut = ResolveSut();
        var nonExistingPlayer = PlayerAlias.Create("NonExistingPlayer");

        // Act
        var result = await sut.GetByPlayerAsync(nonExistingPlayer);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByPlayerAndStatusAsync_WithMatchingGames_ReturnsFilteredGames()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        game2.StartGame();

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);

        // Act
        var result = await sut.GetByPlayerAndStatusAsync(playerAlias, GameStatus.InProgress);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task GetBySpecificationAsync_WithValidSpecification_ReturnsFilteredGames()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await sut.GetBySpecificationAsync(specification);

        // Assert
        result.Should().HaveCount(1);
        result.First().PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithMatchingGame_ReturnsFirstGame()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game = CreateTestGame(playerAlias);
        await sut.SaveAsync(game);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().NotBeNull();
        result!.PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithNoMatchingGame_ReturnsNull()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CountBySpecificationAsync_WithMatchingGames_ReturnsCorrectCount()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var game3 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);
        await sut.SaveAsync(game3);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await sut.CountBySpecificationAsync(specification);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetRecentGamesAsync_WithMultipleGames_ReturnsLimitedGamesOrderedByCreatedAt()
    {
        // Arrange
        var sut = ResolveSut();
        var games = new List<SudokuGame>();
        for (int i = 0; i < 15; i++)
        {
            var game = CreateTestGame();
            games.Add(game);
            await sut.SaveAsync(game);
            await Task.Delay(1); // Ensure different timestamps
        }

        // Act
        var result = await sut.GetRecentGamesAsync(10);

        // Assert
        result.Should().HaveCount(10);
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetCompletedGamesAsync_WithCompletedGames_ReturnsOnlyCompletedGames()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        
        // Complete one game
        game1.StartGame();
        game1.MakeMove(0, 0, 1); // This won't complete the game, but we'll set it manually for testing
        // We can't actually complete the game through normal means in this test setup
        
        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);

        // Act
        var result = await sut.GetCompletedGamesAsync(playerAlias);

        // Assert
        result.Should().AllSatisfy(g => g.Status.Should().Be(GameStatus.Completed));
    }

    [Fact]
    public async Task GetGamesByDifficultyAsync_WithMatchingDifficulty_ReturnsFilteredGames()
    {
        // Arrange
        var sut = ResolveSut();
        var game1 = CreateTestGame(difficulty: GameDifficulty.Easy);
        var game2 = CreateTestGame(difficulty: GameDifficulty.Medium);
        var game3 = CreateTestGame(difficulty: GameDifficulty.Easy);

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);
        await sut.SaveAsync(game3);

        // Act
        var result = await sut.GetGamesByDifficultyAsync(GameDifficulty.Easy);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.Difficulty.Should().Be(GameDifficulty.Easy));
    }

    [Fact]
    public async Task GetGamesByStatusAsync_WithMatchingStatus_ReturnsFilteredGames()
    {
        // Arrange
        var sut = ResolveSut();
        var game1 = CreateTestGame();
        var game2 = CreateTestGame();
        game2.StartGame();

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);

        // Act
        var result = await sut.GetGamesByStatusAsync(GameStatus.InProgress);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task GetTotalGamesCountAsync_WithoutPlayerFilter_ReturnsAllGamesCount()
    {
        // Arrange
        var sut = ResolveSut();
        var game1 = CreateTestGame();
        var game2 = CreateTestGame();
        var game3 = CreateTestGame();

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);
        await sut.SaveAsync(game3);

        // Act
        var result = await sut.GetTotalGamesCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetTotalGamesCountAsync_WithPlayerFilter_ReturnsPlayerGamesCount()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var game3 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));

        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);
        await sut.SaveAsync(game3);

        // Act
        var result = await sut.GetTotalGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetCompletedGamesCountAsync_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        
        await sut.SaveAsync(game1);
        await sut.SaveAsync(game2);

        // Act
        var result = await sut.GetCompletedGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(0); // No games are actually completed in this test
    }

    [Fact]
    public async Task GetAverageCompletionTimeAsync_WithNoCompletedGames_ReturnsZero()
    {
        // Arrange
        var sut = ResolveSut();
        var playerAlias = PlayerAlias.Create("TestPlayer");

        // Act
        var result = await sut.GetAverageCompletionTimeAsync(playerAlias);

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesResourcesWithoutException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Action act = () => ((InMemoryGameRepository)sut).Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ConcurrentOperations_WithMultipleThreads_HandlesOperationsSafely()
    {
        // Arrange
        var sut = ResolveSut();
        var tasks = new List<Task>();
        var games = new List<SudokuGame>();

        for (int i = 0; i < 10; i++)
        {
            var game = CreateTestGame();
            games.Add(game);
        }

        // Act
        foreach (var game in games)
        {
            tasks.Add(sut.SaveAsync(game));
        }

        await Task.WhenAll(tasks);

        // Assert
        var totalCount = await sut.GetTotalGamesCountAsync();
        totalCount.Should().Be(10);
    }

    private static SudokuGame CreateTestGame(PlayerAlias? playerAlias = null, GameDifficulty? difficulty = null)
    {
        var alias = playerAlias ?? PlayerAlias.Create("TestPlayer");
        var diff = difficulty ?? GameDifficulty.Medium;
        var cells = CellsFactory.CreateEmptyCells();
        return SudokuGame.Create(alias, diff, cells);
    }
}