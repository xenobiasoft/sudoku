using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class GameTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame = new();

    public GameTests()
    {
        _mockSudokuGame
            .Setup(x => x.Puzzle)
            .Returns(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddTransient(x => _mockSudokuGame.Object);
    }


}