using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;

namespace UnitTests.Mocks;

public static class MockPuzzleRepositoryExtensions
{
    extension(Mock<IPuzzleRepository> mock)
    {
        public void SaveAsyncThrows(Exception ex)
        {
            mock
                .SetupSequence(x => x.SaveAsync(It.IsAny<SudokuPuzzle>()))
                .ThrowsAsync(ex)
                .Returns(Task.CompletedTask);
        }

        public void UndoReturnsPuzzle(string alias, string puzzleId, SudokuPuzzle puzzle)
        {
            mock
                .Setup(x => x.UndoAsync(alias, puzzleId))
                .ReturnsAsync(puzzle);
        }

        public void VerifySaveAsync(Func<Times> times)
        {
            mock.Verify(x => x.SaveAsync(It.IsAny<SudokuPuzzle>()), times);
        }
    }
}