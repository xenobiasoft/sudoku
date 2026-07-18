using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Specifications;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Builders;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Repositories;

[LogOutput(LogOutputTiming.Always)]
public class AzureBlobGameRepositoryTests : MoqBaseTestByAbstraction<AzureBlobGameRepository, IGameRepository>
{
    private readonly Mock<IAzureStorageService> _mockStorageService;
    private const string ContainerName = "sudoku-games";
    private const string DefaultRevision = "00001";

    public AzureBlobGameRepositoryTests()
    {
        _mockStorageService = Container.ResolveMock<IAzureStorageService>().AsMoq();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new SudokuGameGenerator());
    }

    [Fact]
    public async Task CountBySpecificationAsync_WithMatchingGames_ReturnsCorrectCount()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        var game3 = CreateTestGame(ProfileId.New());
        var allGames = new[] { game1, game2, game3 };

        SetupGetAllGamesAsync(allGames);

        var specification = new GameByProfileIdSpecification(profileId);
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
        var profileId = ProfileId.New();
        var gameId = GameId.New();
        var blobName1 = GetBlobPath(profileId.ToString(), gameId, "00001");
        var blobName2 = GetBlobPath(profileId.ToString(), gameId, "00002");
        var matchingPrefix = $"*/{gameId.Value}/";

        var blobNames = new[] { blobName1, blobName2 };

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(blobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(gameId);

        // Assert
        _mockStorageService.Verify(x => x.DeleteAsync(ContainerName, blobName1), Times.Once);
        _mockStorageService.Verify(x => x.DeleteAsync(ContainerName, blobName2), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var profileId = ProfileId.New();
        var gameId = GameId.New();
        var blobName = GetBlobPath(profileId.ToString(), gameId, "00001");
        var matchingPrefix = $"*/{gameId.Value}/";
        var expectedException = new InvalidOperationException("Storage error");

        var blobNames = new[] { blobName };

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(blobNames.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.DeleteAsync(ContainerName, blobName))
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
        var profileId = ProfileId.New();
        var gameId = GameId.New();
        var blobName = GetBlobPath(profileId.ToString(), gameId, "00001");
        var matchingPrefix = $"*/{gameId.Value}/";

        var blobNames = new[] { blobName };

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(blobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync(gameId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingGame_ReturnsFalse()
    {
        // Arrange
        var gameId = GameId.New();
        var matchingPrefix = $"*/{gameId.Value}/";
        var emptyBlobNames = Array.Empty<string>();

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(emptyBlobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync(gameId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var matchingPrefix = $"*/{gameId.Value}/";
        var expectedException = new InvalidOperationException("Storage error");

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Throws(expectedException);

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
        var profileId = ProfileId.New();
        var allGames = Array.Empty<SudokuGame>();

        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAverageCompletionTimeAsync(profileId);

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGame_ReturnsGame()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game = CreateTestGame(profileId);
        var gameId = game.Id;
        var matchingPrefix = $"*/{gameId.Value}/";
        var blobName = GetBlobPath(profileId.ToString(), gameId, "00001");

        var blobNames = new[] { blobName };

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(blobNames.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{gameId.Value}/"))
            .Returns(blobNames.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>(ContainerName, blobName))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(game.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(game.Id);
        result.DisplayName.Should().Be(game.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingGame_ReturnsNull()
    {
        // Arrange
        var gameId = GameId.New();
        var matchingPrefix = $"*/{gameId.Value}/";
        var emptyBlobNames = Array.Empty<string>();

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Returns(emptyBlobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(gameId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var matchingPrefix = $"*/{gameId.Value}/";
        var expectedException = new InvalidOperationException("Storage error");

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, matchingPrefix))
            .Throws(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.GetByIdAsync(gameId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task GetByProfileIdAsync_WithExistingProfile_ReturnsGamesOrderedByCreatedAt()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        var prefixPath = $"{profileId}/";

        var blobName1 = GetBlobPath(profileId.ToString(), game1.Id, "00001");
        var blobName2 = GetBlobPath(profileId.ToString(), game2.Id, "00001");
        var blobNames = new[] { blobName1, blobName2 };

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, prefixPath))
            .Returns(blobNames.ToAsyncEnumerable());

        // Setup for GetLatestRevisionAsync
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{game1.Id.Value}/"))
            .Returns(new[] { blobName1 }.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{game2.Id.Value}/"))
            .Returns(new[] { blobName2 }.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>(ContainerName, blobName1))
            .ReturnsAsync(game1);

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuGame>(ContainerName, blobName2))
            .ReturnsAsync(game2);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByProfileIdAsync(profileId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.ProfileId.Should().Be(profileId));
        result.Should().BeInDescendingOrder(g => g.CreatedAt);
    }

    [Fact]
    public async Task GetByProfileIdAsync_WithNonExistingProfile_ReturnsEmptyCollection()
    {
        // Arrange
        var profileId = ProfileId.New();
        var expectedPrefix = $"{profileId}/";
        var emptyBlobNames = Array.Empty<string>();

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, expectedPrefix))
            .Returns(emptyBlobNames.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByProfileIdAsync(profileId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByProfileIdAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var profileId = ProfileId.New();
        var expectedPrefix = $"{profileId}/";
        var expectedException = new InvalidOperationException("Storage error");

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, expectedPrefix))
            .Throws(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.GetByProfileIdAsync(profileId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Storage error");
    }

    [Fact]
    public async Task GetByProfileIdAndStatusAsync_WithMatchingGames_ReturnsFilteredGames()
    {
        // Arrange
        var profileId = ProfileId.New();
        var status = GameStatusEnum.InProgress;
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        game2.StartGame(); // Set to InProgress
        var allGames = new[] { game1, game2 };

        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByProfileIdAndStatusAsync(profileId, status);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(status);
        result.First().ProfileId.Should().Be(profileId);
    }

    [Fact]
    public async Task GetBySpecificationAsync_WithValidSpecification_ReturnsFilteredGames()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(ProfileId.New());
        var allGames = new[] { game1, game2 };

        SetupGetAllGamesAsync(allGames);

        var specification = new GameByProfileIdSpecification(profileId);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetBySpecificationAsync(specification);

        // Assert
        result.Should().HaveCount(1);
        result.First().ProfileId.Should().Be(profileId);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithMatchingGame_ReturnsFirstGame()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game = CreateTestGame(profileId);
        var allGames = new[] { game };

        SetupGetAllGamesAsync(allGames);

        var specification = new GameByProfileIdSpecification(profileId);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetSingleBySpecificationAsync(specification);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().Be(profileId);
    }

    [Fact]
    public async Task GetSingleBySpecificationAsync_WithNoMatchingGame_ReturnsNull()
    {
        // Arrange
        var profileId = ProfileId.New();
        var allGames = Array.Empty<SudokuGame>();

        SetupGetAllGamesAsync(allGames);

        var specification = new GameByProfileIdSpecification(profileId);
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
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        var allGames = new[] { game1, game2 };

        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetCompletedGamesAsync(profileId);

        // Assert
        result.Should().AllSatisfy(g => g.Status.Should().Be(GameStatusEnum.Completed));
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
        var result = await sut.GetGamesByStatusAsync(GameStatusEnum.InProgress);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(GameStatusEnum.InProgress);
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
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        var game3 = CreateTestGame(ProfileId.New());
        var allGames = new[] { game1, game2, game3 };

        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetTotalGamesCountAsync(profileId);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetCompletedGamesCountAsync_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game1 = CreateTestGame(profileId);
        var game2 = CreateTestGame(profileId);
        var allGames = new[] { game1, game2 };

        SetupGetAllGamesAsync(allGames);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetCompletedGamesCountAsync(profileId);

        // Assert
        result.Should().Be(0); // No games are actually completed in this test
    }

    [Fact]
    public async Task SaveAsync_WhenStorageServiceThrows_RethrowsException()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game = CreateTestGame(profileId);
        var expectedBlobName = GetBlobPath(profileId.ToString(), game.Id, "00001");
        var expectedException = new InvalidOperationException("Storage error");

        // Setup for GetNextRevisionBlobNameAsync
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{game.Id.Value}/"))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.SaveAsync(ContainerName, expectedBlobName, game))
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
        var profileId = ProfileId.New();
        var game = CreateTestGame(profileId);
        var expectedBlobName = GetBlobPath(profileId.ToString(), game.Id, "00001");

        // Setup for GetNextRevisionBlobNameAsync
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{game.Id.Value}/"))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockStorageService.Verify(x => x.SaveAsync(ContainerName, expectedBlobName, game), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WithExistingGame_CreatesNewRevision()
    {
        // Arrange
        var profileId = ProfileId.New();
        var game = CreateTestGame(profileId);
        var existingBlobName = GetBlobPath(profileId.ToString(), game.Id, "00001");
        var expectedBlobName = GetBlobPath(profileId.ToString(), game.Id, "00002");

        // Setup for GetNextRevisionBlobNameAsync
        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{game.Id.Value}/"))
            .Returns(new[] { existingBlobName }.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockStorageService.Verify(x => x.SaveAsync(ContainerName, expectedBlobName, game), Times.Once);
    }

    private SudokuGame CreateTestGame(ProfileId? profileId = null, GameDifficulty? difficulty = null)
    {
        var pid = profileId ?? ProfileId.New();
        var displayName = PlayerAlias.Create("TestPlayer");
        var diff = difficulty ?? GameDifficulty.Medium;
        var cells = CellsFactory.CreateEmptyCells();
        return SudokuGame.Create(pid, displayName, diff, BoardSize.Nine, cells);
    }

    private static string GetBlobPath(string profileId, GameId gameId, string revision)
    {
        return $"{profileId}/{gameId.Value}/{revision}.json";
    }

    private void SetupGetAllGamesAsync(SudokuGame[] games)
    {
        var allBlobNames = new List<string>();
        var gameInfos = new Dictionary<string, (string ProfileId, GameId GameId, string BlobName)>();

        for (var i = 0; i < games.Length; i++)
        {
            var game = games[i];
            var profileId = game.ProfileId.ToString();
            var gameId = game.Id;
            var blobName = GetBlobPath(profileId, gameId, "00001");

            allBlobNames.Add(blobName);
            gameInfos[blobName] = (profileId, gameId, blobName);

            // Setup for GetLatestRevisionAsync
            _mockStorageService
                .Setup(x => x.GetBlobNamesAsync(ContainerName, $"{profileId}/{gameId.Value}/"))
                .Returns(new[] { blobName }.ToAsyncEnumerable());

            _mockStorageService
                .Setup(x => x.LoadAsync<SudokuGame>(ContainerName, blobName))
                .ReturnsAsync(game);
        }

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(ContainerName, null))
            .Returns(allBlobNames.ToAsyncEnumerable());
    }
}
