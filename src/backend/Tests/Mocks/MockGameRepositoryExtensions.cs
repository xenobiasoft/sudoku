using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Mocks;

public static class MockGameRepositoryExtensions
{
    public static void SetupGameStarted(this Mock<IGameRepository> mock)
    {
        var game = GameFactory.CreateStartedGame();

        mock
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);
    }

    public static void SetupInvalidGame(this Mock<IGameRepository> mock)
    {
        var game = GameFactory.CreateInvalidGame();

        mock
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(game);
    }
}