using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;

namespace UnitTests.Web.Components;

public class GameBoardTests : TestContext
{
    public GameBoardTests()
    {
        var mockNotificationService = new Mock<ICellFocusedNotificationService>();
        Services.AddSingleton(mockNotificationService.Object);
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
        cellInputs.ToList().ForEach(x => x.MarkupMatches("<td class=\"cell\"><input class=\"\" type=\"text\" maxlength=\"1\"></td>"));
    }
}