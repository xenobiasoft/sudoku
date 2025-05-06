using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Components;

public class CellInputTests : TestContext
{
    private readonly Mock<ICellFocusedNotificationService> _mockCellFocusNotifier = new();
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotificationService = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();
    private readonly Mock<IGameStateManager> _mockGameStorageManager = new();

    public CellInputTests()
    {
        Services.AddSingleton(_mockCellFocusNotifier.Object);
        Services.AddSingleton(_mockInvalidCellNotificationService.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
        Services.AddSingleton(_mockGameStorageManager.Object);
    }

	[Fact]
	public void RenderingCellInput_WhenCellIsLocked_RendersLabel()
	{
		// Arrange
        var cell = new Cell(0, 0)
        {
			Locked = true,
			Value = 4
        };

		// Act
        var renderComponent = RenderComponent<CellInput>(x => x.Add(p => p.Cell, cell));

		// Assert
        renderComponent.MarkupMatches("<td class=\"cell\"><label class=\"\">4</label></td>");
    }

    [Fact]
    public void RenderingCellInput_WhenCellIsNotLocked_RendersInput()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            Locked = false,
            Value = 7
        };

        // Act
        var renderedComponent = RenderComponent<CellInput>(x => x.Add(p => p.Cell, cell));

        // Assert
        renderedComponent.MarkupMatches("<td class=\"cell\"><input class=\"\" type=\"text\" maxlength=\"1\" value=\"7\" readonly=\"\" /></td>");
    }

    [Fact]
    public async Task CellFocusNotify_WhenCellIsInRowColumnOrMiniGrid_HighlightsCell()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            Locked = false,
            Value = 7
        };
        var renderedCell = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell));
        _mockCellFocusNotifier
            .Setup(x => x.Notify(cell))
            .Raises(x => x.SetCellFocus += null, this, cell);

        // Act
        await renderedCell.InvokeAsync(() => _mockCellFocusNotifier.Object.Notify(cell));
        renderedCell.Render();

        // Assert
        renderedCell.MarkupMatches("<td class=\"cell\"><input class=\"highlight\" type=\"text\" maxlength=\"1\" value=\"7\" readonly=\"\" /></td>");
    }

    [Fact]
    public async Task InvalidCellNotify_WhenCellIncludedInInvalidListOfCells_MarksInvalid()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            Locked = false,
            Value = 7
        };
        var invalidCells = new List<Cell> { cell };
        var renderedCell = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell));
        _mockInvalidCellNotificationService
            .Setup(x => x.Notify(invalidCells))
            .Raises(x => x.NotifyInvalidCells += null, this, invalidCells);

        // Act
        await renderedCell.InvokeAsync(() => _mockInvalidCellNotificationService.Object.Notify(invalidCells));
        renderedCell.Render();

        // Assert
        renderedCell.MarkupMatches("<td class=\"cell\"><input class=\"invalid\" type=\"text\" maxlength=\"1\" value=\"7\" readonly=\"\" /></td>");
    }

    [Fact]
    public void CellInput_WhenPuzzleSolved_SendsGameEndedNotification()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var cell = puzzle.GetCell(0, 0);
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell)
            .Add(p => p.Puzzle, puzzle));

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad2);
        cellInput.Find("input").KeyPress(Key.NumberPad1);

        // Assert
        _mockGameNotificationService.Verify(x => x.NotifyGameEnded(), Times.Once);
    }

    [Fact]
    public async Task CellInput_WhenValueEntered_SavesGameState()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var cell = puzzle.GetCell(0, 0);
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell)
            .Add(p => p.Puzzle, puzzle));

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad4);

        // Assert
        _mockGameStorageManager.VerifySaveAsyncCalled(Times.Once);
    }
}