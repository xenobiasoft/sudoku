using DepenMock.Attributes;
using DepenMock.Moq;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;
using Handler = Sudoku.Application.Handlers.DeleteGameCommandHandler;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class DeleteGameCommandHandlerTests : MoqBaseTestByAbstraction<Handler, ICommandHandler<DeleteGameCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IGameCompletionRepository> _mockCompletionRepository;
    private readonly Mock<ILogger<Handler>> _mockLogger;

    public DeleteGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
        _mockCompletionRepository = Container.ResolveMock<IGameCompletionRepository>().AsMoq();
        _mockLogger = Container.ResolveMock<ILogger<Handler>>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new DeleteGameCommand(gameId);
        var game = CreateTestGame();

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);

        _mockGameRepository.Setup(x => x.DeleteAsync(It.IsAny<GameId>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenGameIsCompletedAndHasNoRecord_UpsertsCompletionRecordBeforeDeleting()
    {
        // Arrange — the backstop: a won game must never be deleted without its win being recorded.
        var game = GameFactory.CreateCompletedGame();
        _mockGameRepository.SetupGetById(game);
        _mockCompletionRepository.SetupCompletionNotFound();
        var sut = ResolveSut();

        // Act
        await sut.Handle(new DeleteGameCommand(game.Id.ToString()), CancellationToken.None);

        // Assert
        _mockCompletionRepository.VerifyUpserted(
            new GameCompletion(
                game.Id.ToString(),
                game.ProfileId.ToString(),
                game.Difficulty.Name,
                game.Statistics.PlayDuration,
                game.CompletedAt!.Value,
                game.Size.Size),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenGameIsCompletedAndRecordAlreadyExists_DoesNotWriteAgain()
    {
        // Arrange — the event handler already recorded this win
        var game = GameFactory.CreateCompletedGame();
        _mockGameRepository.SetupGetById(game);
        _mockCompletionRepository.SetupCompletionExists(new GameCompletion(
            game.Id.ToString(),
            game.ProfileId.ToString(),
            game.Difficulty.Name,
            game.Statistics.PlayDuration,
            game.CompletedAt!.Value,
            game.Size.Size));
        var sut = ResolveSut();

        // Act
        await sut.Handle(new DeleteGameCommand(game.Id.ToString()), CancellationToken.None);

        // Assert
        _mockCompletionRepository.VerifyNeverUpserted();
    }

    [Fact]
    public async Task Handle_WhenGameIsNotCompleted_DoesNotWriteACompletionRecord()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();
        _mockGameRepository.SetupGetById(game);
        _mockCompletionRepository.SetupCompletionNotFound();
        var sut = ResolveSut();

        // Act
        await sut.Handle(new DeleteGameCommand(game.Id.ToString()), CancellationToken.None);

        // Assert
        _mockCompletionRepository.VerifyNeverUpserted();
    }

    [Fact]
    public async Task Handle_WhenGameIsCompletedAndCompletionWriteFails_DoesNotDeleteTheGame()
    {
        // Arrange — abandoning the delete preserves the game document so the win can be retried.
        var game = GameFactory.CreateCompletedGame();
        _mockGameRepository.SetupGetById(game);
        _mockCompletionRepository.SetupCompletionNotFound();
        _mockCompletionRepository.SetupUpsertThrows(new InvalidOperationException("Cosmos is down"));
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new DeleteGameCommand(game.Id.ToString()), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockGameRepository.VerifyDeleteNeverCalled();
    }

    private static SudokuGame CreateTestGame()
    {
        var profileId = ProfileId.New();
        var displayName = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var cells = new List<Cell>();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells.Add(Cell.CreateEmpty(i, j, BoardSize.Nine));
            }
        }

        return SudokuGame.Create(profileId, displayName, difficulty, BoardSize.Nine, cells);
    }
}