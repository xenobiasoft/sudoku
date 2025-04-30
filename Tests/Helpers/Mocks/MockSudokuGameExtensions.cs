using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockSudokuGameExtensions
{
    public static Mock<ISudokuGame> SetPuzzle(this Mock<ISudokuGame> mock, ISudokuPuzzle puzzle)
    {
        var memento = new GameStateMemento(Guid.NewGuid().ToString(), puzzle.GetAllCells(), 0);

        return mock.SetPuzzle(memento);
    }

    public static Mock<ISudokuGame> SetPuzzle(this Mock<ISudokuGame> mock, GameStateMemento memento)
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(memento);

        return mock;
    }
}