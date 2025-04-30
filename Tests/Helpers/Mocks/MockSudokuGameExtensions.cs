using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockSudokuGameExtensions
{
    public static Mock<ISudokuGame> SetPuzzle(this Mock<ISudokuGame> mock, ISudokuPuzzle puzzle)
    {
        var memento = new GameStateMemory(Guid.NewGuid().ToString(), puzzle.GetAllCells(), 0);

        return mock.SetPuzzle(memento);
    }

    public static Mock<ISudokuGame> SetPuzzle(this Mock<ISudokuGame> mock, GameStateMemory memory)
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(memory);

        return mock;
    }
}