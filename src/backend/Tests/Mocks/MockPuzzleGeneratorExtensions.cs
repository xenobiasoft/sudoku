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
            mock.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupGeneratePuzzleAsyncReturnsNull()
        {
            mock.Setup(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync((SudokuPuzzle)null!);
        }

        public void VerifyGeneratePuzzleAsyncCalledOnce(GameDifficulty difficulty)
        {
            mock.Verify(x => x.GeneratePuzzleAsync(difficulty), Times.Once);
        }

        public void VerifyGeneratePuzzleAsyncNeverCalled()
        {
            mock.Verify(x => x.GeneratePuzzleAsync(It.IsAny<GameDifficulty>()), Times.Never);
        }
    }
}