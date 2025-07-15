using DepenMock.XUnit;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Application.Handlers;

public class CreateGameCommandHandlerTests : BaseTestByAbstraction<CreateGameCommandHandler, ICommandHandler<CreateGameCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPuzzleRepository> _mockPuzzleRepository;

    public CreateGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();
        _mockPuzzleRepository = Container.ResolveMock<IPuzzleRepository>();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(playerAlias, difficulty);

        _mockPuzzleRepository.Setup(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(puzzle);

        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockPuzzleRepository.Verify(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()), Times.Once);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoPuzzleAvailable_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var command = new CreateGameCommand(playerAlias, difficulty);

        _mockPuzzleRepository.Setup(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()))
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
    public async Task Handle_WithInvalidPlayerAlias_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = ""; // Invalid player alias
        var difficulty = "Medium";
        var command = new CreateGameCommand(playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _mockPuzzleRepository.Verify(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()), Times.Never);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidDifficulty_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "InvalidDifficulty";
        var command = new CreateGameCommand(playerAlias, difficulty);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        _mockPuzzleRepository.Verify(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()), Times.Never);
        _mockGameRepository.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(playerAlias, difficulty);
        var exceptionMessage = "Database error";

        _mockPuzzleRepository.Setup(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(puzzle);

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
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var command = new CreateGameCommand(playerAlias, difficulty);
        var domainException = new InvalidPlayerAliasException("Invalid player alias");

        _mockPuzzleRepository.Setup(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()))
            .ThrowsAsync(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var puzzle = CreateTestPuzzle();
        var command = new CreateGameCommand(playerAlias, difficulty);
        var expectedDifficulty = GameDifficulty.FromName(difficulty);

        _mockPuzzleRepository.Setup(x => x.GetRandomByDifficultyAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(puzzle);

        _mockGameRepository.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        _mockPuzzleRepository.Verify(x => x.GetRandomByDifficultyAsync(
            It.Is<GameDifficulty>(d => d.Name == expectedDifficulty.Name)), Times.Once);
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