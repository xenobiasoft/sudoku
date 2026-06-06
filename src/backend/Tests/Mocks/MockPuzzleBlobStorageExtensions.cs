using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Mocks;

public static class MockPuzzleBlobStorageExtensions
{
    extension(Mock<IPuzzleBlobStorage> mock)
    {
        public void SetupCreateAsyncReturns(SudokuPuzzle puzzle)
        {
            mock.Setup(x => x.CreateAsync(It.IsAny<GameDifficulty>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupGetPuzzleIdAsyncReturns(IEnumerable<string> ids)
        {
            mock.Setup(x => x.GetPuzzleIdsAsync(It.IsAny<string>()))
                .Returns(ids.ToAsyncEnumerable());
        }

        public void SetupGetPuzzleIdsAsyncReturns(IEnumerable<string> ids)
        {
            mock.Setup(x => x.GetPuzzleIdsAsync(It.IsAny<string>()))
                .Returns(ids.ToAsyncEnumerable());
        }

        public void SetupGetPuzzleIdsAsyncReturnsEmpty()
        {
            mock.Setup(x => x.GetPuzzleIdsAsync(It.IsAny<string>()))
                .Returns(Array.Empty<string>().ToAsyncEnumerable());
        }

        public void SetupLoadAsyncReturns(SudokuPuzzle puzzle)
        {
            mock.Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(puzzle);
        }

        public void SetupLoadAsyncReturnsNull()
        {
            mock.Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((SudokuPuzzle)null!);
        }

        public void VerifyCreateAsyncCalledExactly(GameDifficulty difficulty, int times)
        {
            mock.Verify(x => x.CreateAsync(difficulty), Times.Exactly(times));
        }

        public void VerifyCreateAsyncNeverCalled()
        {
            mock.Verify(x => x.CreateAsync(It.IsAny<GameDifficulty>()), Times.Never);
        }

        public void VerifyDeleteAsyncCalledOnce(string prefix, string puzzleId)
        {
            mock.Verify(x => x.DeleteAsync(prefix, puzzleId), Times.Once);
        }

        public void VerifyDeleteAsyncNeverCalled()
        {
            mock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        public void VerifyLoadAsyncCalledOnce(string prefix, string puzzleId)
        {
            mock.Verify(x => x.LoadAsync(prefix, puzzleId), Times.Once);
        }

        public void VerifyLoadAsyncNeverCalled()
        {
            mock.Verify(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}