using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class RequestHintCommandHandlerTests : MoqBaseTestByAbstraction<RequestHintCommandHandler, ICommandHandler<RequestHintCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPuzzleSolver> _mockPuzzleSolver;

    public RequestHintCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
        _mockPuzzleSolver = Container.ResolveMock<IPuzzleSolver>().AsMoq();

        _mockPuzzleSolver
            .Setup(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()))
            .ReturnsAsync(PuzzleFactory.GetSolvedPuzzle());
    }

    [Fact]
    public async Task Handle_WithHintsAvailable_ReturnsSuccessAndSavesGame()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();
        var command = new RequestHintCommand(game.Id.Value.ToString(), TimeSpan.FromSeconds(30));

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>())).ReturnsAsync(game);
        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockPuzzleSolver.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Once);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenGameNotFound_ReturnsFailureAndDoesNotSolve()
    {
        // Arrange
        var command = new RequestHintCommand(Guid.NewGuid().ToString(), TimeSpan.Zero);
        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>())).ReturnsAsync((SudokuGame?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Game not found with ID: {command.GameId}");
        _mockPuzzleSolver.Verify(x => x.SolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Never);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoEmptyCells_ReturnsFailureAndDoesNotSave()
    {
        // Arrange - a fully solved board has no cells to reveal
        var game = GameFactory.CreateGameWithCells(CellsFactory.CreateCompletedCells());
        game.StartGame();
        var command = new RequestHintCommand(game.Id.Value.ToString(), TimeSpan.Zero);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>())).ReturnsAsync(game);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailureResult()
    {
        // Arrange
        var game = GameFactory.CreateGameInProgress();
        var command = new RequestHintCommand(game.Id.Value.ToString(), TimeSpan.Zero);

        _mockGameRepository.Setup(x => x.GetByIdAsync(It.IsAny<GameId>())).ReturnsAsync(game);
        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>())).ThrowsAsync(new Exception("Database error"));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("An unexpected error occurred: Database error");
    }
}
