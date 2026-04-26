using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Mocks;

public static class MockGameRepositoryExtensions
{
    extension(Mock<IGameRepository> mock)
    {
        public void SetupGameStarted()
        {
            var game = GameFactory.CreateStartedGame();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupInvalidGame()
        {
            var game = GameFactory.CreateInvalidGame();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }
    }
}