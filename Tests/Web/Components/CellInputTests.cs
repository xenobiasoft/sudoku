using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Components;

public class CellInputTests : TestContext
{
    private readonly Mock<ICellFocusedNotificationService> _mockCellFocusNotifier = new();
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotificationService = new();

    public CellInputTests()
    {
        Services.AddSingleton(_mockCellFocusNotifier.Object);
        Services.AddSingleton(_mockInvalidCellNotificationService.Object);
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
        renderedComponent.MarkupMatches("<td class=\"cell\"><input class=\"\" type=\"text\" maxlength=\"1\" value=\"7\" /></td>");
    }

    [Fact]
    public async Task GivenCell_WhenNotifiedToSetFocus_HighlightsCell()
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
        renderedCell.MarkupMatches("<td class=\"cell\"><input class=\"highlight\" type=\"text\" maxlength=\"1\" value=\"7\" /></td>");
    }

    [Fact]
    public async Task GivenCell_WhenNotifiedCellInvalid_MarksInvalid()
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
        renderedCell.MarkupMatches("<td class=\"cell\"><input class=\"invalid\" type=\"text\" maxlength=\"1\" value=\"7\" /></td>");
    }
}