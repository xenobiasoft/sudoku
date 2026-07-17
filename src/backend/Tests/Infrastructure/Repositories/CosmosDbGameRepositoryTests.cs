using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Builders;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Repositories;

[LogOutput(LogOutputTiming.Always)]
public class CosmosDbGameRepositoryTests : MoqBaseTestByAbstraction<CosmosDbGameRepository, IGameRepository>
{
    private readonly Mock<ICosmosDbService> _mockCosmosDbService;
    private readonly Mock<IDomainEventDispatcher> _mockEventDispatcher;

    public CosmosDbGameRepositoryTests()
    {
        _mockCosmosDbService = Container.ResolveMock<ICosmosDbService>().AsMoq();
        _mockEventDispatcher = Container.ResolveMock<IDomainEventDispatcher>().AsMoq();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new SudokuGameGenerator());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteItemAsync()
    {
        // Arrange
        var gameId = GameId.New();
        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(gameId);

        // Assert
        _mockCosmosDbService.VerifyDeleteItemAsync(gameId, Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenGameExists_ShouldReturnTrue()
    {
        // Arrange
        var gameId = GameId.New();
        _mockCosmosDbService.ExistsAsyncReturns(gameId, true);
        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync(gameId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenGameDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var gameId = GameId.New();
        _mockCosmosDbService.GetItemReturnsNothing(gameId);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(gameId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenGameExists_ShouldReturnGame()
    {
        // Arrange
        var gameId = GameId.New();
        var profileId = ProfileId.New();
        var displayName = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Easy;
        var gameDocument = new SudokuGameDocument
        {
            Id = gameId.Value.ToString(),
            GameId = gameId.Value.ToString(),
            ProfileId = profileId.ToString(),
            DisplayName = displayName.Value,
            Difficulty = difficulty,
            Status = GameStatusEnum.NotStarted,
            Cells = [],
            CreatedAt = DateTime.UtcNow
        };
        _mockCosmosDbService.GetItemReturnsDocument(gameId, gameDocument);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetByIdAsync(gameId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Value.ToString().Should().Be(gameId.Value.ToString());
        result.DisplayName.Should().Be(displayName);
        result.Difficulty.Should().Be(difficulty);
    }

    [Fact]
    public async Task GetByProfileIdAsync_WithMatchingGames_ShouldReturnProfileGames()
    {
        // Arrange
        var profileId = ProfileId.New();
        var displayName = PlayerAlias.Create("TestPlayer");
        var documents = new List<SudokuGameDocument>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                GameId = Guid.NewGuid().ToString(),
                ProfileId = profileId.ToString(),
                DisplayName = displayName.Value,
                Difficulty = GameDifficulty.Easy,
                Status = GameStatusEnum.NotStarted,
                Cells = [],
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                GameId = Guid.NewGuid().ToString(),
                ProfileId = profileId.ToString(),
                DisplayName = displayName.Value,
                Difficulty = GameDifficulty.Medium,
                Status = GameStatusEnum.InProgress,
                Cells = [],
                CreatedAt = DateTime.UtcNow
            }
        };
        _mockCosmosDbService.GetByProfileIdAsyncReturnsDocuments(profileId.ToString(), documents);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetByProfileIdAsync(profileId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.ProfileId.Should().Be(profileId));
    }

    [Fact]
    public async Task SaveAsync_ShouldCallUpsertItemAsync()
    {
        // Arrange
        var profileId = ProfileId.New();
        var displayName = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Easy;
        var cells = CellsFactory.CreateEmptyCells();
        var game = SudokuGame.Create(profileId, displayName, difficulty, cells);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockCosmosDbService.VerifyUpsertItemAsync(game.Id, displayName.Value, difficulty, Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ShouldDispatchAccumulatedDomainEvents()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockEventDispatcher.VerifyDispatched<GameCreatedEvent>(Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ShouldClearDomainEventsAfterDispatching()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        game.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_WhenSavedTwice_DoesNotRedispatchTheSameEvents()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        var sut = ResolveSut();
        await sut.SaveAsync(game);

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockEventDispatcher.VerifyDispatchedEventCount(0, Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WhenDispatcherThrows_DoesNotThrow()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        _mockEventDispatcher.SetupDispatchThrows(new InvalidOperationException("Handler blew up"));
        var sut = ResolveSut();

        // Act
        var saveAsync = async () => await sut.SaveAsync(game);

        // Assert
        await saveAsync.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveAsync_WhenDispatcherThrows_LogsError()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        _mockEventDispatcher.SetupDispatchThrows(new InvalidOperationException("Handler blew up"));
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        Logger.ErrorLogs().ContainsMessage($"Error dispatching domain events for game {game.Id.Value}");
    }

    [Fact]
    public async Task SaveAsync_WhenUpsertThrows_DoesNotDispatchDomainEvents()
    {
        // Arrange
        var game = GameFactory.CreateEmptyGame();
        _mockCosmosDbService.SetupUpsertThrows(new InvalidOperationException("Cosmos is down"));
        var sut = ResolveSut();

        // Act
        var saveAsync = async () => await sut.SaveAsync(game);

        // Assert
        await saveAsync.Should().ThrowAsync<InvalidOperationException>();
        _mockEventDispatcher.VerifyDispatched<GameCreatedEvent>(Times.Never);
    }
}
