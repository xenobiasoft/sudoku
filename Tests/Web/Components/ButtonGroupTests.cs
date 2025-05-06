using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Web.Components;

public class ButtonGroupTests : TestContext
{
    private readonly Mock<IGameStateManager> _gameStateManagerMock;

    public ButtonGroupTests()
    {
        _gameStateManagerMock = new Mock<IGameStateManager>();
        Services.AddSingleton(_gameStateManagerMock.Object);
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

        // Act
        var numberButton = RenderComponent<ButtonGroup>().Find($"#btn{number}");

        // Assert
		numberButton.MarkupMatches($"<button type=\"button\" id=\"btn{number}\" class=\"btn btn-primary\"><i class=\"fa-solid fa-{number}\"></i></button>");
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
    public void WhenButtonClicked_SetsValueToNumber(int? expected)
	{
		// Arrange
        CellValueChangedEventArgs? calledEventArgs = null;
		var buttonGroup = RenderComponent<ButtonGroup>(p => p
            .Add(x => x.OnNumberClicked, (i) => calledEventArgs = i));
        var button = buttonGroup.Find($"#btn{expected ?? 0}");

		// Act
		button.Click();

		// Assert
        calledEventArgs!.Value.Should().Be(expected);
    }

    [Fact]
    public void UndoAsync_WhenClicked_CallsGameStateManagerUndo()
    {
        // Arrange
        var undoCalled = false;
        var undoButton = RenderComponent<ButtonGroup>(p =>
            {
                p.Add(x => x.PuzzleId, "puzzleId");
                p.Add(x => x.OnUndo, () => undoCalled = true);
            })
            .Find("#btnUndo");

        // Act
        undoButton.Click();

        // Assert
        undoCalled.Should().BeTrue();
    }
}