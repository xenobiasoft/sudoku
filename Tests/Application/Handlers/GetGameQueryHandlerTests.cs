using DepenMock.XUnit;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Application.Handlers;

public class GetGameQueryHandlerTests : BaseTestByAbstraction<GetGameQueryHandler, IQueryHandler<GetGameQuery, GameDto>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public GetGameQueryHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();
    }

    [Fact]
    public async Task Handle_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGame();
        var query = new GetGameQuery(gameId);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(game.Id.Value.ToString());
        result.Value.PlayerAlias.Should().Be(game.PlayerAlias.Value);
        result.Value.Difficulty.Should().Be(game.Difficulty.Name);
        result.Value.Status.Should().Be(game.Status.ToString());
    }

    [Fact]
    public async Task Handle_WhenGameNotFound_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var query = new GetGameQuery(gameId);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync((SudokuGame?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Game not found with ID: {gameId}");
    }

    [Fact]
    public async Task Handle_WithInvalidGameId_ReturnsFailureResult()
    {
        // Arrange
        var invalidGameId = "invalid-guid";
        var query = new GetGameQuery(invalidGameId);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var query = new GetGameQuery(gameId);
        var exceptionMessage = "Database error";

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureWithDomainMessage()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var query = new GetGameQuery(gameId);
        var domainException = new InvalidPlayerAliasException("Invalid game ID");

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ThrowsAsync(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectGameId()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGame();
        var query = new GetGameQuery(gameId);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        await sut.Handle(query, CancellationToken.None);

        // Assert
        _mockGameRepository.Verify(x => x.GetByIdAsync(
            It.Is<GameId>(id => id.Value.ToString() == gameId)), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectGameDto()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var game = CreateTestGame();
        var query = new GetGameQuery(gameId);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        
        var gameDto = result.Value!;
        gameDto.Id.Should().Be(game.Id.Value.ToString());
        gameDto.PlayerAlias.Should().Be(game.PlayerAlias.Value);
        gameDto.Difficulty.Should().Be(game.Difficulty.Name);
        gameDto.Status.Should().Be(game.Status.ToString());
        gameDto.CreatedAt.Should().Be(game.CreatedAt);
        gameDto.StartedAt.Should().Be(game.StartedAt);
        gameDto.CompletedAt.Should().Be(game.CompletedAt);
        gameDto.PausedAt.Should().Be(game.PausedAt);
        gameDto.Statistics.Should().NotBeNull();
        gameDto.Cells.Should().NotBeNull();
        gameDto.Cells.Count.Should().Be(game.GetCells().Count);
    }

    private static SudokuGame CreateTestGame()
    {
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var cells = new List<Cell>();
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells.Add(Cell.CreateEmpty(i, j));
            }
        }

        return SudokuGame.Create(playerAlias, difficulty, cells);
    }
}