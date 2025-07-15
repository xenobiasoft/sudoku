using DepenMock.XUnit;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using InvalidMoveException = Sudoku.Domain.Exceptions.InvalidMoveException;

namespace UnitTests.Application.Handlers;

public class MakeMoveCommandHandlerTests : BaseTestByAbstraction<MakeMoveCommandHandler, ICommandHandler<MakeMoveCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public MakeMoveCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();
    }

    [Fact]
    public async Task Handle_WithValidMove_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var game = CreateTestGameInProgress();
        var command = new MakeMoveCommand(gameId, row, column, value);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockGameRepository.Verify(x => x.GetByIdAsync(It.IsAny<GameId>()), Times.Once);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenGameNotFound_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var command = new MakeMoveCommand(gameId, row, column, value);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync((SudokuGame?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Game not found with ID: {gameId}");
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidGameId_ReturnsFailureResult()
    {
        // Arrange
        var invalidGameId = "invalid-guid";
        var row = 0;
        var column = 0;
        var value = 5;
        var command = new MakeMoveCommand(invalidGameId, row, column, value);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenGameThrowsInvalidMoveException_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var command = new MakeMoveCommand(gameId, row, column, value);

        // Create a game with a cell that already has value 5, so attempting to place 5 in the same row will fail
        var game = CreateTestGameWithDuplicateValue();

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var game = CreateTestGameInProgress();
        var command = new MakeMoveCommand(gameId, row, column, value);
        var exceptionMessage = "Database error";

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectGameId()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var game = CreateTestGameInProgress();
        var command = new MakeMoveCommand(gameId, row, column, value);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        _mockGameRepository.Verify(x => x.GetByIdAsync(
            It.Is<GameId>(id => id.Value.ToString() == gameId)), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsGameMakeMoveWithCorrectParameters()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 2;
        var column = 3;
        var value = 7;
        var command = new MakeMoveCommand(gameId, row, column, value);

        var game = CreateTestGameInProgress();
        var initialCellValue = game.GetCell(row, column).Value;

        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        _mockGameRepository
            .Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert - Verify that the cell value was updated
        game.GetCell(row, column).Value.Should().Be(value);
        game.GetCell(row, column).Value.Should().NotBe(initialCellValue);
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureWithDomainMessage()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var row = 0;
        var column = 0;
        var value = 5;
        var command = new MakeMoveCommand(gameId, row, column, value);
        var domainException = new GameNotInProgressException(GameId.Create(gameId));

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ThrowsAsync(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    private static SudokuGame CreateTestGameInProgress()
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

        var game = SudokuGame.Create(playerAlias, difficulty, cells);
        game.StartGame();
        return game;
    }

    private static SudokuGame CreateTestGameWithDuplicateValue()
    {
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var cells = new List<Cell>();
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                // Create a cell with value 5 at position (0, 1) to create a duplicate scenario
                var cell = i == 0 && j == 1 ? Cell.Create(i, j, 5, false) : Cell.CreateEmpty(i, j);
                cells.Add(cell);
            }
        }

        var game = SudokuGame.Create(playerAlias, difficulty, cells);
        game.StartGame();
        return game;
    }
}