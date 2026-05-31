using DepenMock.Attributes;
using DepenMock.Moq;
using DepenMock.XUnit.V3.Attributes;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using LogOutput = DepenMock.XUnit.V3.Attributes.LogOutputAttribute;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class CreateGameCommandHandlerTests : MoqBaseTestByAbstraction<CreateGameCommandHandler, ICommandHandler<CreateGameCommand, string>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPuzzleGenerator> _mockPuzzleGenerator;
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;

    public CreateGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
        _mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>().AsMoq();
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();
    }

    [Fact]
    public async Task Handle_WhenPoolHasPuzzle_UsesPuzzleFromPoolAndSkipsGenerator()
    {
        // Arrange
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");

        _mockPuzzlePoolService.SetupDequeueReturns(puzzle);
        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockPuzzlePoolService.VerifyDequeueCalledOnce();
        _mockPuzzleGenerator.Verify(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPoolEmpty_FallsBackToGeneratorAndLogsWarning()
    {
        // Arrange
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");

        _mockPuzzlePoolService.SetupDequeueReturnsEmpty();
        _mockPuzzleGenerator.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>())).ReturnsAsync(puzzle);
        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockPuzzlePoolService.VerifyDequeueCalledOnce();
        _mockPuzzleGenerator.Verify(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()), Times.Once);
        Logger.WarningLogs().ContainsMessage("Puzzle pool empty");
    }

    [Fact]
    public async Task Handle_WhenPoolEmptyAndGeneratorReturnsNull_ReturnsFailure()
    {
        // Arrange
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");

        _mockPuzzlePoolService.SetupDequeueReturnsEmpty();
        _mockPuzzleGenerator.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync((SudokuPuzzle?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No puzzle available for difficulty: Medium");
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");

        _mockPuzzlePoolService.SetupDequeueReturns(puzzle);
        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidDisplayName_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "", "Medium");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _mockPuzzlePoolService.VerifyDequeueNotCalled();
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidDifficulty_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "InvalidDifficulty");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _mockPuzzlePoolService.VerifyDequeueNotCalled();
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");
        var exceptionMessage = "Database error";

        _mockPuzzlePoolService.SetupDequeueReturns(puzzle);
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
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureWithDomainMessage()
    {
        // Arrange
        var command = new CreateGameCommand(Guid.NewGuid().ToString(), "TestPlayer", "Medium");
        var domainException = new InvalidPlayerAliasException("Invalid player alias");

        _mockPuzzlePoolService.Setup(x => x.DequeueAsync(It.IsAny<GameDifficulty>()))
            .ThrowsAsync(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    private static SudokuPuzzle CreateTestPuzzle()
    {
        var cells = new List<Cell>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells.Add(Cell.CreateEmpty(i, j));
            }
        }

        return SudokuPuzzle.Create("test-puzzle-id", GameDifficulty.Medium, cells);
    }
}
