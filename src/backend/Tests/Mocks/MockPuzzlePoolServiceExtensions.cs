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

        public void VerifyDequeueCalledOnce()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<GameDifficulty>()), Times.Once);
        }

        public void VerifyDequeueNotCalled()
        {
            mock.Verify(x => x.DequeueAsync(It.IsAny<GameDifficulty>()), Times.Never);
        }
    }
}
