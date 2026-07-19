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
                .Setup(x => x.DequeueAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupDequeueReturnsEmpty()
        {
            mock
                .Setup(x => x.DequeueAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()))
                .ReturnsAsync((SudokuPuzzle?)null);
        }

        public void SetupDequeueThrows(Exception ex)
        {
            mock.Setup(x => x.DequeueAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()))
                .ThrowsAsync(ex);
        }

        public void SetupGetAvailableCountReturns(int count)
        {
            mock.Setup(x => x.GetAvailableCountAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()))
                .ReturnsAsync(count);
        }

        public void SetupGetAvailableCountReturns(BoardSize size, GameDifficulty difficulty, int count)
        {
            mock.Setup(x => x.GetAvailableCountAsync(size, difficulty))
                .ReturnsAsync(count);
        }

        public void VerifyDequeueCalledOnce()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()), Times.Once);
        }

        public void VerifyDequeueNotCalled()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>()), Times.Never);
        }

        public void VerifyGetAvailableCountCalledOnce(BoardSize size, GameDifficulty difficulty)
        {
            mock.Verify(x => x.GetAvailableCountAsync(size, difficulty), Times.Once);
        }

        public void VerifySeedCalledOnce(BoardSize size, GameDifficulty difficulty, int expectedCount)
        {
            mock.Verify(x => x.SeedAsync(size, difficulty, expectedCount), Times.Once);
        }

        public void VerifySeedCalledTimes(BoardSize size, GameDifficulty difficulty, int expectedCount, int times)
        {
            mock.Verify(x => x.SeedAsync(size, difficulty, expectedCount), Times.Exactly(times));
        }

        public void VerifySeedNeverCalled()
        {
            mock.Verify(x => x.SeedAsync(It.IsAny<BoardSize>(), It.IsAny<GameDifficulty>(), It.IsAny<int>()), Times.Never);
        }
    }
}
