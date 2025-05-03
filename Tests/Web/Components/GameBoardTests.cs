using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Components;

public class GameBoardTests : TestContext
{
    public GameBoardTests()
    {
        Services.AddSingleton(new Mock<ICellFocusedNotificationService>().Object);
        Services.AddSingleton(new Mock<IInvalidCellNotificationService>().Object);
        Services.AddSingleton(new Mock<IGameNotificationService>().Object);
        Services.AddSingleton(new Mock<IGameStateManager>().Object);
    }

	[Fact]
	public void GameBoard_RendersCellsCorrectly()
	{
		// Arrange
        var gameBoard = RenderComponent<GameBoard>(p => p
            .Add(x => x.Puzzle, PuzzleFactory.GetEmptyPuzzle()));

		// Act
        var cellInputs = gameBoard.FindComponents<CellInput>();

        // Assert
        cellInputs.Count.Should().Be(81);
        cellInputs.ToList().ForEach(x => x.MarkupMatches("<td class=\"cell\"><input class=\"\" type=\"text\" maxlength=\"1\" readonly=\"\"></td>"));
    }
}