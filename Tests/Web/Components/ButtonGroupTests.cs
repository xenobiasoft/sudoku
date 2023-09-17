using Sudoku.Web.Server.Components;

namespace UnitTests.Web.Components;

public class ButtonGroupTests : TestContext
{
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
    public void WhenButtonClicked_SetsValueToNumber(int expected)
	{
		// Arrange
        var actual = 0;
		var buttonGroup = RenderComponent<ButtonGroup>(p => p
            .Add(x => x.NumberClicked, (i) => actual = i));
        var button = buttonGroup.Find($"#btn{expected}");

		// Act
		button.Click();

		// Assert
        actual.Should().Be(expected);
    }
}