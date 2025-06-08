using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockSudokuGameExtensions
{
    public static Mock<ISudokuGame> SetLoadAsync(this Mock<ISudokuGame> mock, ISudokuPuzzle puzzle)
    {
        var gameState = new GameStateMemory { Board = puzzle.GetAllCells(), PuzzleId = Guid.NewGuid().ToString() };

        return mock.SetLoadAsync(gameState);
    }

    public static Mock<ISudokuGame> SetLoadAsync(this Mock<ISudokuGame> mock, GameStateMemory memory)
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(memory);

        return mock;
    }

    public static Mock<ISudokuGame> SetNewAsync(this Mock<ISudokuGame> mock, string alias, ISudokuPuzzle puzzle)
    {
        var gameState = new GameStateMemory
        {
            Alias = alias,
            Board = puzzle.GetAllCells(), 
            PuzzleId = Guid.NewGuid().ToString()
        };

        return mock.SetNewAsync(alias, gameState);
    }

    public static Mock<ISudokuGame> SetNewAsync(this Mock<ISudokuGame> mock, string alias, GameStateMemory memory)
    {
        mock
            .Setup(x => x.NewGameAsync(alias, It.IsAny<Level>()))
            .ReturnsAsync(memory);

        return mock;
    }

    public static Mock<ISudokuGame> VerifyGeneratesNewPuzzle(this Mock<ISudokuGame> mock, Func<Times> times)
    {
        mock.Verify(x => x.NewGameAsync(It.IsAny<string>(), It.IsAny<Level>()), times);

        return mock;
    }

    public static Mock<ISudokuGame> VerifyLoadsAsync(this Mock<ISudokuGame> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadAsync(alias, puzzleId), times);

        return mock;
    }
}