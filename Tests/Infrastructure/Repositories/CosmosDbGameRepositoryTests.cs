using DepenMock.XUnit;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Builders;
using UnitTests.Helpers.Factories;
using UnitTests.Mocks;

namespace UnitTests.Infrastructure.Repositories;

public class CosmosDbGameRepositoryTests : BaseTestByAbstraction<CosmosDbGameRepository, IGameRepository>
{
    private readonly Mock<ICosmosDbService> _mockCosmosDbService;

    public CosmosDbGameRepositoryTests()
    {
        _mockCosmosDbService = Container.ResolveMock<ICosmosDbService>();
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
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Easy;
        var gameDocument = new SudokuGameDocument
        {
            Id = gameId.Value.ToString(),
            GameId = gameId.Value.ToString(),
            PlayerAlias = playerAlias.Value,
            Difficulty = difficulty,
            Status = GameStatus.NotStarted,
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
        result.PlayerAlias.Should().Be(playerAlias);
        result.Difficulty.Should().Be(difficulty);
    }

    [Fact]
    public async Task GetByPlayerAsync_WithMatchingGames_ShouldReturnPlayerGames()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var documents = new List<SudokuGameDocument>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                GameId = Guid.NewGuid().ToString(),
                PlayerAlias = playerAlias.Value,
                Difficulty = GameDifficulty.Easy,
                Status = GameStatus.NotStarted,
                Cells = [],
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
            new()
            {
                Id = Guid.NewGuid().ToString(), 
                GameId = Guid.NewGuid().ToString(),
                PlayerAlias = playerAlias.Value,
                Difficulty = GameDifficulty.Medium,
                Status = GameStatus.InProgress,
                Cells = [],
                CreatedAt = DateTime.UtcNow
            }
        };
        _mockCosmosDbService.GetByPlayerAsyncReturnsDocuments(playerAlias, documents);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetByPlayerAsync(playerAlias);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.PlayerAlias.Should().Be(playerAlias));
    }

    [Fact]
    public async Task SaveAsync_ShouldCallUpsertItemAsync()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var difficulty = GameDifficulty.Easy;
        var cells = CellsFactory.CreateEmptyCells();
        var game = SudokuGame.Create(playerAlias, difficulty, cells);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(game);

        // Assert
        _mockCosmosDbService.VerifyUpsertItemAsync(game.Id, playerAlias, difficulty, Times.Once);
    }
}