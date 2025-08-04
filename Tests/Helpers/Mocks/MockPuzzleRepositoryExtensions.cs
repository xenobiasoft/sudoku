using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;

namespace UnitTests.Helpers.Mocks;

public static class MockPuzzleRepositoryExtensions
{
    public static void SaveAsyncThrows(this Mock<IPuzzleRepository> mock, Exception ex)
    {
        mock
            .SetupSequence(x => x.SaveAsync(It.IsAny<SudokuPuzzle>()))
            .ThrowsAsync(ex)
            .Returns(Task.CompletedTask);
    }

    public static void UndoReturnsPuzzle(this Mock<IPuzzleRepository> mock, string alias, string puzzleId, SudokuPuzzle puzzle)
    {
        mock
            .Setup(x => x.UndoAsync(alias, puzzleId))
            .ReturnsAsync(puzzle);
    }

    public static void VerifySaveAsync(this Mock<IPuzzleRepository> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<SudokuPuzzle>()), times);
    }
}