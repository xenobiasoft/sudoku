using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Mocks;

public static class MockPuzzlePoolServiceExtensions
{
    extension(Mock<IPuzzlePoolService> mock)
    {
        public void SetupDequeueReturns(SudokuPuzzle? puzzle)
        {
            mock
                .Setup(x => x.DequeueAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupDequeueReturnsEmpty()
        {
            mock
                .Setup(x => x.DequeueAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync((SudokuPuzzle?)null);
        }

        public void SetupDequeueThrows(Exception ex)
        {
            mock.Setup(x => x.DequeueAsync(It.IsAny<GameDifficulty>()))
                .ThrowsAsync(ex);
        }

        public void SetupGetAvailableCountReturns(int count)
        {
            mock.Setup(x => x.GetAvailableCountAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync(count);
        }

        public void VerifyDequeueCalledOnce()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<GameDifficulty>()), Times.Once);
        }

        public void VerifyDequeueNotCalled()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<GameDifficulty>()), Times.Never);
        }

        public void VerifyGetAvailableCountCalledOnce(GameDifficulty difficulty)
        {
            mock.Verify(x => x.GetAvailableCountAsync(difficulty), Times.Once);
        }

        public void VerifySeedCalledOnce(GameDifficulty difficulty, int expectedCount)
        {
            mock.Verify(x => x.SeedAsync(difficulty, expectedCount), Times.Once);
        }

        public void VerifySeedNeverCalled()
        {
            mock.Verify(x => x.SeedAsync(It.IsAny<GameDifficulty>(), It.IsAny<int>()), Times.Never);
        }
    }
}
