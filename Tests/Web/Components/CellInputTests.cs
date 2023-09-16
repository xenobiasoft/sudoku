using Sudoku.Web.Server.Components;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Components;

public class CellInputTests : TestContext
{
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
        renderComponent.MarkupMatches("<td class=\"cell\"><label>4</label></td>");
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
        renderedComponent.MarkupMatches("<td class=\"cell\"><input type=\"text\" maxlength=\"1\" value=\"7\" /></td>");
    }
}