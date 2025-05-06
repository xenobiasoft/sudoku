using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

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

    [Fact]
    public async Task GameBoard_ShouldBubbleUp_CellChangedEvent()
    {
        // Arrange
        CellChangedEventArgs? calledArgs = null;
        var gameBoard = RenderComponent<GameBoard>(x => x
            .Add(c => c.OnCellChanged, args => calledArgs = args)
            .Add(c => c.Puzzle, PuzzleFactory.GetPuzzle(Level.Easy)));
        var cellInput = gameBoard.FindComponents<CellInput>().First();

        // Act
        await cellInput.InvokeAsync(() => cellInput.Instance.OnCellChanged.InvokeAsync(new CellChangedEventArgs(1, 2, 5)));

        // Assert
        Assert.Multiple(() =>
        {
            calledArgs.Should().NotBeNull();
            calledArgs!.Row.Should().Be(1);
            calledArgs.Column.Should().Be(2);
            calledArgs.Value.Should().Be(5);
        });
    }
}