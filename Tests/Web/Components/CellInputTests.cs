using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services.Abstractions;
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

    public CellInputTests()
    {
        Services.AddSingleton(_mockCellFocusNotifier.Object);
        Services.AddSingleton(_mockInvalidCellNotificationService.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
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
        renderComponent.MarkupMatches("<td class=\"cell\"><label tabindex=\"0\" class=\"\">4</label></td>");
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
        renderedComponent.MarkupMatches(@"
                                           <td class=""cell"">
                                              <div class=""pencil-values"">
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                              </div>
                                              <input class="""" type=""text"" maxlength=""1"" readonly="""" value=""7"">
                                          </td>");
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
        renderedCell.MarkupMatches(@"
                                     <td class=""cell"">
                                         <div class=""pencil-values"">
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                         </div>
                                         <input class=""highlight"" type=""text"" maxlength=""1"" readonly="""" value=""7"">
                                     </td>");
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
        renderedCell.MarkupMatches(@"
                                      <td class=""cell"">
                                         <div class=""pencil-values"">
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                             <span class=""pencil-entry""></span>
                                         </div>
                                         <input class=""invalid"" type=""text"" maxlength=""1"" readonly="""" value=""7"">
                                     </td>");
    }

    [Fact]
    public void CellInput_WhenValueChangedNotInPencilMode_RaisesCellChangedEvent()
    {
        // Arrange
        var calledArgs = (CellChangedEventArgs)null!;
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(c => c.Cell, new Cell(1, 2))
            .Add(c => c.Puzzle, PuzzleFactory.GetPuzzle(Level.Easy))
            .Add(c => c.OnCellChanged, args => calledArgs = args)
            .Add(c => c.IsPencilMode, false));

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad5);

        // Assert
        Assert.Multiple(() =>
        {
            calledArgs.Should().NotBeNull();
            calledArgs.Row.Should().Be(1);
            calledArgs.Column.Should().Be(2);
            calledArgs.Value.Should().Be(5);
        });
    }

    [Fact]
    public void CellInput_WhenValueChangedInPencilMode_RaisesCellChangedEvent()
    {
        // Arrange
        var calledArgs = (CellPossibleValueChangedEventArgs)null!;
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(c => c.Cell, new Cell(1, 2))
            .Add(c => c.Puzzle, PuzzleFactory.GetPuzzle(Level.Easy))
            .Add(c => c.OnPossibleValueChanged, args => calledArgs = args)
            .Add(c => c.IsPencilMode, true));

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad5);

        // Assert
        Assert.Multiple(() =>
        {
            calledArgs.Should().NotBeNull();
            calledArgs.Row.Should().Be(1);
            calledArgs.Column.Should().Be(2);
            calledArgs.Value.Should().Be(5);
        });
    }

    [Fact]
    public void CellInput_WhenPossibleNumbersEntered_DisplaysInCell()
    {
        // Arrange
        var cell = new Cell(0, 0)
        {
            Locked = false,
            PossibleValues = [1,2,7]
        };

        // Act
        var renderedComponent = RenderComponent<CellInput>(x => x.Add(p => p.Cell, cell));

        // Assert
        renderedComponent.MarkupMatches(@"
                                           <td class=""cell"">
                                              <div class=""pencil-values"">
                                                  <span class=""pencil-entry"">1</span>
                                                  <span class=""pencil-entry"">2</span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry"">7</span>
                                                  <span class=""pencil-entry""></span>
                                                  <span class=""pencil-entry""></span>
                                              </div>
                                              <input class="""" type=""text"" maxlength=""1"" readonly="""">
                                          </td>");
    }

    [Fact]
    public void CellInput_WhenValueEnteredInCellWithPossibleNumbers_ClearsPossibleNumbers()
    {
        // Arrange
        var expectedBefore = @"
           <td class=""cell"">
              <div class=""pencil-values"">
                  <span class=""pencil-entry"">1</span>
                  <span class=""pencil-entry"">2</span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry"">7</span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
              </div>
              <input class="""" type=""text"" maxlength=""1"" readonly="""">
          </td>";
        var expectedAfter = @"
           <td class=""cell"">
              <div class=""pencil-values"">
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
                  <span class=""pencil-entry""></span>
              </div>
              <input class="""" type=""text"" maxlength=""1"" readonly="""" value=""7"">
          </td>";
        var cell = new Cell(0, 0)
        {
            Locked = false,
            PossibleValues = [1, 2, 7]
        };
        var cellInput = RenderComponent<CellInput>(x => x.Add(p => p.Cell, cell));
        cellInput.MarkupMatches(expectedBefore);

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad7);

        // Assert
        cellInput.MarkupMatches(expectedAfter);
    }
}