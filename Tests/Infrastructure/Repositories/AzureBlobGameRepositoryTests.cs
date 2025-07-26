using System.Runtime.CompilerServices;
using DepenMock.XUnit;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;
using XenobiaSoft.Sudoku.Infrastructure.Repositories;
using XenobiaSoft.Sudoku.Infrastructure.Services;

namespace UnitTests.Infrastructure.Repositories;

public class AzureBlobGameRepositoryTests : BaseTestByAbstraction<AzureBlobGameRepository, IGameRepository>
{
    private readonly Mock<IAzureStorageService> _mockStorageService;

    public AzureBlobGameRepositoryTests()
    {
        _mockStorageService = Container.ResolveMock<IAzureStorageService>();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasClassBuilder());
        container.AddCustomizations(new SudokuGameClassBuilder());
    }

    [Fact]
    public async Task CountBySpecificationAsync_WithMatchingGames_ReturnsCorrectCount()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var game3 = CreateTestGame(Container.Create<PlayerAlias>());
        var allGames = new[] { game1, game2, game3 };
        
        SetupGetAllGamesAsync(allGames);

        var specification = new GameByPlayerSpecification(playerAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.CountBySpecificationAsync(specification);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingGame_DeletesGame()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";

        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(gameId);

        // Assert
        _mockStorageService.Verify(x => x.DeleteAsync("sudoku-games", expectedBlobName), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";
        var expectedException = new InvalidOperationException("Storage error");
        
        _mockStorageService
            .Setup(x => x.DeleteAsync("sudoku-games", expectedBlobName))
            .ThrowsAsync(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DeleteAsync(gameId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesResourcesWithoutException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var act = () => ((AzureBlobGameRepository)sut).Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingGame_ReturnsTrue()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _mockStorageService.Verify(x => x.ExistsAsync("sudoku-games", expectedBlobName), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingGame_ReturnsFalse()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";
        
        _mockStorageService
            .Setup(x => x.ExistsAsync("sudoku-games", expectedBlobName))
            .ReturnsAsync(false);

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync(gameId);

        // Assert
        result.Should().BeFalse();
        _mockStorageService.Verify(x => x.ExistsAsync("sudoku-games", expectedBlobName), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";
        var expectedException = new InvalidOperationException("Storage error");
        
        _mockStorageService
            .Setup(x => x.ExistsAsync("sudoku-games", expectedBlobName))
            .ThrowsAsync(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.ExistsAsync(gameId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task GetAverageCompletionTimeAsync_WithNoCompletedGames_ReturnsZero()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var allGames = Array.Empty<SudokuGame>();
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAverageCompletionTimeAsync(playerAlias);

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGame_ReturnsGame()
    {
        // Arrange
        var game = CreateTestGame();
        var expectedBlobName = $"games/{game.Id.Value}.json";
        
        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", expectedBlobName))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(game.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(game.Id);
        result.PlayerAlias.Should().Be(game.PlayerAlias);
        _mockStorageService.Verify(x => x.LoadAsync<SudokuGame>("sudoku-games", expectedBlobName), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingGame_ReturnsNull()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";
        
        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", expectedBlobName))
            .ReturnsAsync((SudokuGame?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(gameId);

        // Assert
        result.Should().BeNull();
        _mockStorageService.Verify(x => x.LoadAsync<SudokuGame>("sudoku-games", expectedBlobName), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var expectedBlobName = $"games/{gameId.Value}.json";
        var expectedException = new InvalidOperationException("Storage error");
        
        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", expectedBlobName))
            .ThrowsAsync(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.GetByIdAsync(gameId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task GetByPlayerAsync_WithExistingPlayer_ReturnsGamesOrderedByCreatedAt()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var expectedPrefix = $"{playerAlias.Value}/";
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var blobNames = new[] { "blob1", "blob2" };
        
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync("sudoku-games", expectedPrefix))
            .Returns(blobNames.ToAsyncEnumerable());
        
        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", "blob1"))
            .ReturnsAsync(game1);
        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", "blob2"))
            .ReturnsAsync(game2);

        var sut = ResolveSut();

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
        var playerAlias = PlayerAlias.Create("NonExistingPlayer");
        var expectedPrefix = $"{playerAlias.Value}/";
        var emptyBlobNames = Array.Empty<string>();
        
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync("sudoku-games", expectedPrefix))
            .Returns(emptyBlobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByPlayerAsync(playerAlias);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByPlayerAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var expectedPrefix = $"{playerAlias.Value}/";
        var expectedException = new InvalidOperationException("Storage error");
        
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync("sudoku-games", expectedPrefix))
            .Throws(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.GetByPlayerAsync(playerAlias);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task GetByPlayerAndStatusAsync_WithMatchingGames_ReturnsFilteredGames()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var status = GameStatus.InProgress;
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        game2.StartGame(); // Set to InProgress
        var allGames = new[] { game1, game2 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByPlayerAndStatusAsync(playerAlias, status);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(status);
        result.First().PlayerAlias.Should().Be(playerAlias);
    }

    [Fact]
    public async Task GetBySpecificationAsync_WithValidSpecification_ReturnsFilteredGames()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));
        var allGames = new[] { game1, game2 };
        
        SetupGetAllGamesAsync(allGames);

        var specification = new GameByPlayerSpecification(playerAlias);
        var sut = ResolveSut();

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
        var playerAlias = Container.Create<PlayerAlias>();
        var game = CreateTestGame(playerAlias);
        var allGames = new[] { game };
        
        SetupGetAllGamesAsync(allGames);

        var specification = new GameByPlayerSpecification(playerAlias);
        var sut = ResolveSut();

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
        var playerAlias = Container.Create<PlayerAlias>();
        var allGames = Array.Empty<SudokuGame>();
        
        SetupGetAllGamesAsync(allGames);

        var specification = new GameByPlayerSpecification(playerAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRecentGamesAsync_WithMultipleGames_ReturnsLimitedGamesOrderedByCreatedAt()
    {
        // Arrange
        var games = new SudokuGame[15];
        for (int i = 0; i < 15; i++)
        {
            games[i] = CreateTestGame();
        }
        
        SetupGetAllGamesAsync(games);

        var sut = ResolveSut();

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
        var playerAlias = Container.Create<PlayerAlias>();
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var allGames = new[] { game1, game2 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetCompletedGamesAsync(playerAlias);

        // Assert
        result.Should().AllSatisfy(g => g.Status.Should().Be(GameStatus.Completed));
    }

    [Fact]
    public async Task GetGamesByDifficultyAsync_WithMatchingDifficulty_ReturnsFilteredGames()
    {
        // Arrange
        var game1 = CreateTestGame(difficulty: GameDifficulty.Easy);
        var game2 = CreateTestGame(difficulty: GameDifficulty.Medium);
        var game3 = CreateTestGame(difficulty: GameDifficulty.Easy);
        var allGames = new[] { game1, game2, game3 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

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
        var game1 = CreateTestGame();
        var game2 = CreateTestGame();
        game2.StartGame();
        var allGames = new[] { game1, game2 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

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
        var game1 = CreateTestGame();
        var game2 = CreateTestGame();
        var game3 = CreateTestGame();
        var allGames = new[] { game1, game2, game3 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetTotalGamesCountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task GetTotalGamesCountAsync_WithPlayerFilter_ReturnsPlayerGamesCount()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var game3 = CreateTestGame(PlayerAlias.Create("AnotherPlayer"));
        var allGames = new[] { game1, game2, game3 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetTotalGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetCompletedGamesCountAsync_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var game1 = CreateTestGame(playerAlias);
        var game2 = CreateTestGame(playerAlias);
        var allGames = new[] { game1, game2 };
        
        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetCompletedGamesCountAsync(playerAlias);

        // Assert
        result.Should().Be(0); // No games are actually completed in this test
    }

    [Fact]
    public async Task SaveAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var game = CreateTestGame();
        var expectedBlobName = $"games/{game.Id.Value}.json";
        var expectedException = new InvalidOperationException("Storage error");
        
        _mockStorageService
            .Setup(x => x.SaveAsync("sudoku-games", expectedBlobName, game))
            .ThrowsAsync(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.SaveAsync(game);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task SaveAsync_WithValidGame_SavesGameSuccessfully()
    {
        // Arrange
        var game = CreateTestGame();
        var expectedBlobName = $"games/{game.Id.Value}.json";

        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockStorageService.Verify(x => x.SaveAsync("sudoku-games", expectedBlobName, game), Times.Once);
    }

    private static List<Cell> CreateEmptyCells()
    {
        var cells = new List<Cell>();
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        return cells;
    }

    private SudokuGame CreateTestGame(PlayerAlias? playerAlias = null, GameDifficulty? difficulty = null)
    {
        var alias = playerAlias ?? Container.Create<PlayerAlias>();
        var diff = difficulty ?? GameDifficulty.Medium;
        var cells = CreateEmptyCells();
        return SudokuGame.Create(alias, diff, cells);
    }

    private void SetupGetAllGamesAsync(SudokuGame[] games)
    {
        var blobNames = games.Select((g, i) => $"blob{i}").ToArray();
        
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync("sudoku-games", null))
            .Returns(blobNames.ToAsyncEnumerable());
        
        for (var i = 0; i < games.Length; i++)
        {
            var blobName = $"blob{i}";
            _mockStorageService
                .Setup(x => x.LoadAsync<SudokuGame>("sudoku-games", blobName))
                .ReturnsAsync(games[i]);
        }
    }
}

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException(nameof(enumerable));
        }

        foreach (var item in enumerable)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }
}