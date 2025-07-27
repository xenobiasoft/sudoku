using DepenMock.XUnit;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Handler = Sudoku.Application.Handlers.DeleteGameCommandHandler;

namespace UnitTests.Application.Handlers;

public class DeleteGameCommandHandlerTests : BaseTestByAbstraction<Handler, ICommandHandler<DeleteGameCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<ILogger<Handler>> _mockLogger;

    public DeleteGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();
        _mockLogger = Container.ResolveMock<ILogger<Handler>>();
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