using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Mocks;

public static class MockGameRepositoryExtensions
{
    extension(Mock<IGameRepository> mock)
    {
        public void SetupGameInProgress()
        {
            var game = GameFactory.CreateGameInProgress();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGetById(SudokuGame game)
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGameNotFound()
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync((SudokuGame?)null);
        }

        public void SetupGameNotStarted()
        {
            var game = GameFactory.CreateGameNotStarted();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGamePaused()
        {
            var game = GameFactory.CreatePausedGame();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGameStarted()
        {
            var game = GameFactory.CreateStartedGame();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGameWithPossibleValue(int row, int column, int value)
        {
            var game = GameFactory.CreateGameWithPossibleValue(row, column, value);

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupGetByIdThrows(Exception ex)
        {
            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ThrowsAsync(ex);
        }

        public void SetupGetByProfileIdAndStatus(IEnumerable<SudokuGame> games)
        {
            mock
                .Setup(x => x.GetByProfileIdAndStatusAsync(It.IsAny<ProfileId>(), It.IsAny<GameStatusEnum>()))
                .ReturnsAsync(games.ToList());
        }

        public void SetupGetByProfileId(IEnumerable<SudokuGame> games)
        {
            mock
                .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
                .ReturnsAsync(games.ToList());
        }

        public void SetupGetByProfileIdThrows(Exception ex)
        {
            mock
                .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
                .ThrowsAsync(ex);
        }

        public void SetupInvalidGame()
        {
            var game = GameFactory.CreateInvalidGame();

            mock
                .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
                .ReturnsAsync(game);
        }

        public void SetupSaveThrows(Exception ex)
        {
            mock.Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
                .ThrowsAsync(ex);
        }

        public void VerifySaveCalledOnce()
        {
            mock.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Once);
        }

        public void VerifySaveNeverCalled()
        {
            mock.Verify(x => x.SaveAsync(It.IsAny<SudokuGame>()), Times.Never);
        }

        public void VerifyDeleteNeverCalled()
        {
            mock.Verify(x => x.DeleteAsync(It.IsAny<GameId>()), Times.Never);
        }
    }
}