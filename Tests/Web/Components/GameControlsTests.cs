using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests.Web.Components;

public class GameControlsTests : TestContext
{
    private readonly Mock<IGameStateManager> _mockGameStateManager = new();

    public GameControlsTests()
    {
        Services.AddSingleton(_mockGameStateManager.Object);
    }

    [Theory]
	[InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void NumberButton_WhenClickedInPencilMode_RaisesEventWithValue(int? expected)
	{
		// Arrange
        CellPossibleValueChangedEventArgs? calledEventArgs = null;
		var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.IsPencilMode, true);
            p.Add(x => x.OnPossibleValueChanged, (i) => calledEventArgs = i);
        });
        var button = gameControls.Find($"#btn{expected ?? 0}");

		// Act
		button.Click();

		// Assert
        calledEventArgs!.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void NumberButton_WhenClickedNotInPencilMode_RaisesEventWithValue(int? expected)
    {
        // Arrange
        CellValueChangedEventArgs? calledEventArgs = null;
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.IsPencilMode, false);
            p.Add(x => x.OnValueChanged, (i) => calledEventArgs = i);
        });
        var button = gameControls.Find($"#btn{expected ?? 0}");

        // Act
        button.Click();

        // Assert
        calledEventArgs!.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(false, "btn btn-outline-primary")]
    [InlineData(true, "btn btn-primary")]
    public void PencilModeButton_RendersCorrectMode(bool isPencilMode, string cssClass)
    {
        // Arrange
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.IsPencilMode, isPencilMode);
        });

        // Act
        var pencilModeButton = gameControls.Find("#btnPencilMode");

        // Assert
        pencilModeButton.MarkupMatches($"<button type=\"button\" id=\"btnPencilMode\" class=\"{cssClass}\">Possible Values <i class=\"fa-solid fa-pencil\"></i></button>");
    }

    [Fact]
    public void PencilModeButton_WhenClicked_SendsButtonModeArgument()
    {
        // Arrange
        var isPencilMode = false;
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.OnPencilMode, (i) => isPencilMode = i);
        });
        var pencilModeButton = gameControls.Find("#btnPencilMode");

        // Act
        pencilModeButton.Click();

        // Assert
        isPencilMode.Should().BeTrue();
        pencilModeButton.Click();
        isPencilMode.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void RenderComponent_RendersNumbersCorrectly(int number)
    {
        // Arrange
        var gameControls = RenderComponent<GameControls>();

        // Act
        var numberButton = gameControls.Find($"#btn{number}");

        // Assert
        numberButton.MarkupMatches($"<button type=\"button\" id=\"btn{number}\" class=\"btn btn-primary btn-num\"><i class=\"fa-solid fa-{number}\"></i></button>");
    }

    [Fact]
    public void ResetButton_WhenClicked_ResetsGameBoard()
    {
        // Arrange
        var resetCalled = false;
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.OnReset, () => resetCalled = true);
        });
        var resetButton = gameControls.Find("#btnReset");

        // Act
        resetButton.Click();

        // Assert
        resetCalled.Should().BeTrue();
    }

    [Fact]
    public void UndoAsync_WhenClicked_CallsGameStateManagerUndo()
    {
        // Arrange
        var undoCalled = false;
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.OnUndo, () => undoCalled = true);
        });
        var undoButton = gameControls.Find("#btnUndo");

        // Act
        undoButton.Click();

        // Assert
        undoCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    public void UndoAsync_WhenOnInitialGameState_IsDisabled(int totalMoves, bool disabled)
    {
        // Arrange
        var gameControls = RenderComponent<GameControls>(p =>
        {
            p.Add(x => x.PuzzleId, "puzzleId");
            p.Add(x => x.OnUndo, () => { });
            p.Add(x => x.TotalMoves, totalMoves);
        });

        // Act
        var undoButton = gameControls.Find("#btnUndo");

        // Assert
        undoButton.HasAttribute("disabled").Should().Be(disabled);
    }
}