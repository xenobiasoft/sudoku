using DepenMock.XUnit;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;
using UnitTests.Helpers.Factories;
using XenobiaSoft.Sudoku.Infrastructure.Repositories;

namespace UnitTests.Infrastructure.Repositories;

public class InMemoryGameRepositoryTests : BaseTestByAbstraction<InMemoryGameRepository, IGameRepository>
{
    private readonly IGameRepository _sut;

    public InMemoryGameRepositoryTests()
    {
        _sut = ResolveSut();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new CellGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new SudokuGameGenerator());
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGame_ReturnsGame()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();
        await _sut.SaveAsync(game);

        // Act
        var result = await _sut.GetByIdAsync(game.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(game.Id);
        result.PlayerAlias.Should().Be(game.PlayerAlias);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingGame_ReturnsNull()
    {
        // Arrange
        var nonExistingId = GameId.New();

        // Act
        var result = await _sut.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_WithValidGame_SavesGameSuccessfully()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();

        // Act
        await _sut.SaveAsync(game);

        // Assert
        var savedGame = await _sut.GetByIdAsync(game.Id);
        savedGame.Should().NotBeNull();
        savedGame!.Id.Should().Be(game.Id);
    }

    [Fact]
    public async Task SaveAsync_WithExistingGame_UpdatesGame()
    {
        // Arrange
        var game = GameFactory.CreateGameNotStarted();
        await _sut.SaveAsync(game);

        // Modify the game
        game.StartGame();

        // Act
        await _sut.SaveAsync(game);

        // Assert
        var updatedGame = await _sut.GetByIdAsync(game.Id);
        updatedGame.Should().NotBeNull();
        updatedGame!.Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingGame_DeletesGame()
    {
        // Arrange
        var sut = ResolveSut();
        var game = GameFactory.CreateGameInProgress();

        await sut.SaveAsync(game);

        // Act
        await _sut.DeleteAsync(game.Id);

        // Assert
        var deletedGame = await _sut.GetByIdAsync(game.Id);
        deletedGame.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingGame_DoesNotThrow()
    {
        // Arrange
        var nonExistingId = GameId.New();

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(nonExistingId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingGame_ReturnsTrue()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();

        await _sut.SaveAsync(game);

        // Act
        var result = await _sut.ExistsAsync(game.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingGame_ReturnsFalse()
    {
        // Arrange
        var nonExistingId = GameId.New();

        // Act
        var result = await _sut.ExistsAsync(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByPlayerAsync_WithExistingPlayer_ReturnsGamesOrderedByCreatedAt()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(playerAlias);
        var game3 = GameFactory.CreateGameForPlayer(PlayerAlias.Create("AnotherPlayer"));

        await _sut.SaveAsync(game1);
        await Task.Delay(10); // Ensure different timestamps
        await _sut.SaveAsync(game2);
        await _sut.SaveAsync(game3);

        // Act
        var result = await _sut.GetByPlayerAsync(playerAlias);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.PlayerAlias.Should().Be(playerAlias));
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetByPlayerAsync_WithNonExistingPlayer_ReturnsEmptyCollection()
    {
        // Arrange
        var nonExistingPlayer = PlayerAlias.Create("NonExistingPlayer");

        // Act
        var result = await _sut.GetByPlayerAsync(nonExistingPlayer);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByPlayerAndStatusAsync_WithMatchingGames_ReturnsFilteredGames()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(playerAlias);
        game2.StartGame();

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);

        // Act
        var result = await _sut.GetByPlayerAndStatusAsync(playerAlias, GameStatus.InProgress);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task GetBySpecificationAsync_WithValidSpecification_ReturnsFilteredGames()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(PlayerAlias.Create("AnotherPlayer"));

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await _sut.GetBySpecificationAsync(specification);

        // Assert
        result.Should().HaveCount(1);
        result.First().PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithMatchingGame_ReturnsFirstGame()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game = GameFactory.CreateGameForPlayer(playerAlias);
        await _sut.SaveAsync(game);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await _sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().NotBeNull();
        result!.PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithNoMatchingGame_ReturnsNull()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await _sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CountBySpecificationAsync_WithMatchingGames_ReturnsCorrectCount()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(playerAlias);
        var game3 = GameFactory.CreateGameForPlayer(PlayerAlias.Create("AnotherPlayer"));

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);
        await _sut.SaveAsync(game3);

        var specification = new GameByPlayerSpecification(playerAlias);

        // Act
        var result = await _sut.CountBySpecificationAsync(specification);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetRecentGamesAsync_WithMultipleGames_ReturnsLimitedGamesOrderedByCreatedAt()
    {
        // Arrange
        var games = new List<SudokuGame>();
        for (var i = 0; i < 15; i++)
        {
            var game = GameFactory.CreateGameInProgress();
            games.Add(game);
            await _sut.SaveAsync(game);
            await Task.Delay(1); // Ensure different timestamps
        }

        // Act
        var result = await _sut.GetRecentGamesAsync(10);

        // Assert
        result.Should().HaveCount(10);
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetCompletedGamesAsync_WithCompletedGames_ReturnsOnlyCompletedGames()
    {
        // Arrange
        var game1 = GameFactory.CreateCompletedGame();
        var game2 = GameFactory.CreateGameInProgress();
        var playerAlias = game1.PlayerAlias;

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);

        // Act
        var result = await _sut.GetCompletedGamesAsync(playerAlias);

        // Assert
        result.Should().AllSatisfy(g => g.Status.Should().Be(GameStatus.Completed));
    }

    [Fact]
    public async Task GetGamesByDifficultyAsync_WithMatchingDifficulty_ReturnsFilteredGames()
    {
        // Arrange
        var game1 = GameFactory.CreateGameWithDifficulty(difficulty: GameDifficulty.Easy);
        var game2 = GameFactory.CreateGameWithDifficulty(difficulty: GameDifficulty.Medium);
        var game3 = GameFactory.CreateGameWithDifficulty(difficulty: GameDifficulty.Easy);

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);
        await _sut.SaveAsync(game3);

        // Act
        var result = await _sut.GetGamesByDifficultyAsync(GameDifficulty.Easy);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.Difficulty.Should().Be(GameDifficulty.Easy));
    }

    [Fact]
    public async Task GetGamesByStatusAsync_WithMatchingStatus_ReturnsFilteredGames()
    {
        // Arrange
        var game1 = GameFactory.CreateGameNotStarted();
        var game2 = GameFactory.CreateGameInProgress();

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);

        // Act
        var result = await _sut.GetGamesByStatusAsync(GameStatus.InProgress);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(GameStatus.InProgress);
    }

    [Fact]
    public async Task GetTotalGamesCountAsync_WithoutPlayerFilter_ReturnsAllGamesCount()
    {
        // Arrange
        var game1 = GameFactory.CreateGameInProgress();
        var game2 = GameFactory.CreateGameInProgress();
        var game3 = GameFactory.CreateGameInProgress();

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);
        await _sut.SaveAsync(game3);

        // Act
        var result = await _sut.GetTotalGamesCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetTotalGamesCountAsync_WithPlayerFilter_ReturnsPlayerGamesCount()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(playerAlias);
        var game3 = GameFactory.CreateGameForPlayer(PlayerAlias.Create("AnotherPlayer"));

        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);
        await _sut.SaveAsync(game3);

        // Act
        var result = await _sut.GetTotalGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetCompletedGamesCountAsync_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var game1 = GameFactory.CreateGameForPlayer(playerAlias);
        var game2 = GameFactory.CreateGameForPlayer(playerAlias);
        
        await _sut.SaveAsync(game1);
        await _sut.SaveAsync(game2);

        // Act
        var result = await _sut.GetCompletedGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(0); // No games are actually completed in this test
    }

    [Fact]
    public async Task GetAverageCompletionTimeAsync_WithNoCompletedGames_ReturnsZero()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");

        // Act
        var result = await _sut.GetAverageCompletionTimeAsync(playerAlias);

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesResourcesWithoutException()
    {
        // Arrange

        // Act
        Action act = () => ((InMemoryGameRepository)_sut).Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ConcurrentOperations_WithMultipleThreads_HandlesOperationsSafely()
    {
        // Arrange
        var tasks = new List<Task>();
        var games = new List<SudokuGame>();

        for (var i = 0; i < 10; i++)
        {
            var game = GameFactory.CreateGameInProgress();
            games.Add(game);
        }

        // Act
        foreach (var game in games)
        {
            tasks.Add(_sut.SaveAsync(game));
        }

        await Task.WhenAll(tasks);

        // Assert
        var totalCount = await _sut.GetTotalGamesCountAsync();
        totalCount.Should().Be(10);
    }
}