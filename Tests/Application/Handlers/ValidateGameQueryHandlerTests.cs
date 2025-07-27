using DepenMock.XUnit;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;
using UnitTests.Helpers.Factories;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Application.Handlers;

public class ValidateGameQueryHandlerTests : BaseTestByAbstraction<ValidateGameQueryHandler, IQueryHandler<ValidateGameQuery, ValidationResultDto>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly ValidateGameQuery _query;

    public ValidateGameQueryHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();

        _query = new ValidateGameQuery(GameId.New());
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(GameFactory.CreateStartedGame());
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new CellGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new SudokuGameGenerator());
    }

    [Fact]
    public async Task Handle_LogsInformationWithValidationResult()
    {
        // Arrange
        _mockGameRepository.SetupGameStarted();
        var sut = ResolveSut();

        // Act
        await sut.Handle(_query, CancellationToken.None);

        // Assert
        Logger.InformationLogs().ContainsMessage("Validated game with ID");
        Logger.InformationLogs().ContainsMessage("IsValid: True");
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var exceptionMessage = "Invalid game state";
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ThrowsAsync(new InvalidPuzzleException(exceptionMessage));
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task Handle_WithInvalidGame_ReturnsValidationErrorsResult()
    {
        // Arrange
        var errors = new List<string>
        {
            "Row 2 contains duplicate values",
            "Column 2 contains duplicate values",
            "Box at position (1, 1) contains duplicate values"
        };
        _mockGameRepository.SetupInvalidGame();
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeFalse();
        result.Value.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public async Task Handle_WithNonExistentGameId_ReturnsFailureResult()
    {
        // Arrange
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync((SudokuGame)null);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Game not found");
    }

    [Fact]
    public async Task Handle_WithValidGameId_ReturnsValidationResult()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsValid.Should().BeTrue();
        result.Value.Errors.Should().BeEmpty();
    }
}