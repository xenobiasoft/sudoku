using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Mocks;

public static class MockPuzzleGeneratorExtensions
{
    extension(Mock<IPuzzleGenerator> mock)
    {
        public void SetupGeneratePuzzleAsyncReturns(SudokuPuzzle puzzle)
        {
            mock.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>(), It.IsAny<BoardSize>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupGeneratePuzzleAsyncReturnsNull()
        {
            mock.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>(), It.IsAny<BoardSize>()))
                .ReturnsAsync((SudokuPuzzle)null!);
        }

        public void VerifyGeneratePuzzleAsyncCalledOnce(GameDifficulty difficulty, BoardSize size)
        {
            mock.Verify(x => x.GeneratePuzzleAsync(difficulty, size), Times.Once);
        }

        public void VerifyGeneratePuzzleAsyncNeverCalled()
        {
            mock.Verify(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>(), It.IsAny<BoardSize>()), Times.Never);
        }
    }
}
