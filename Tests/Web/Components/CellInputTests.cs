using Microsoft.Extensions.DependencyInjection;
using Sudoku.Domain.ValueObjects;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;
using UnitTests.Helpers.Factories;

namespace UnitTests.Web.Components;

public class CellInputTests : TestContext
{
    private readonly Mock<INotificationService> _mockNotificationService = new();

    public CellInputTests()
    {
        Services.AddSingleton(_mockNotificationService.Object);
    }

    [Fact]
    public async Task CellFocusNotify_WhenCellIsInRowColumnOrMiniGrid_HighlightsCell()
    {
        // Arrange
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = false,
            Value = 7
        };
        var renderedCell = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell));
        _mockNotificationService
            .Setup(x => x.NotifyCellFocused(cell))
            .Raises(x => x.SetCellFocus += null, this, cell);

        // Act
        await renderedCell.InvokeAsync(() => _mockNotificationService.Object.NotifyCellFocused(cell));
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
    public void CellInput_WhenPossibleNumbersEntered_DisplaysInCell()
    {
        // Arrange
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = false,
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
    public void CellInput_WhenValueChangedInPencilMode_RaisesCellChangedEvent()
    {
        // Arrange
        var calledArgs = (CellPossibleValueChangedEventArgs)null!;
        var game = GameModelFactory
            .Build()
            .WithDifficulty(GameDifficulty.Easy)
            .Create();
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(c => c.Cell, new CellModel{ Row = 1, Column = 2 })
            .Add(c => c.CurrentGame, game)
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
    public void CellInput_WhenValueChangedNotInPencilMode_RaisesCellChangedEvent()
    {
        // Arrange
        var calledArgs = (CellChangedEventArgs)null!;
        var game = GameModelFactory
            .Build()
            .WithDifficulty(GameDifficulty.Easy)
            .Create();
        var cellInput = RenderComponent<CellInput>(x => x
            .Add(c => c.Cell, new CellModel { Row = 1, Column = 2 })
            .Add(c => c.CurrentGame, game)
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
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = false,
            PossibleValues = [1, 2, 7]
        };
        var cellInput = RenderComponent<CellInput>(x => x.Add(p => p.Cell, cell));
        cellInput.MarkupMatches(expectedBefore);

        // Act
        cellInput.Find("input").KeyPress(Key.NumberPad7);

        // Assert
        cellInput.MarkupMatches(expectedAfter);
    }

    [Fact]
    public async Task InvalidCellNotify_WhenCellIncludedInInvalidListOfCells_MarksInvalid()
    {
        // Arrange
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = false,
            Value = 7
        };
        var invalidCells = new List<CellModel> { cell };
        var renderedCell = RenderComponent<CellInput>(x => x
            .Add(p => p.Cell, cell));
        _mockNotificationService
            .Setup(x => x.NotifyInvalidCells(invalidCells))
            .Raises(x => x.InvalidCellsNotified += null, this, invalidCells);

        // Act
        await renderedCell.InvokeAsync(() => _mockNotificationService.Object.NotifyInvalidCells(invalidCells));
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
	public void RenderingCellInput_WhenCellIsLocked_RendersLabel()
	{
		// Arrange
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = true,
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
        var cell = new CellModel
        {
            Row = 0,
            Column = 0,
            IsFixed = false,
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
}