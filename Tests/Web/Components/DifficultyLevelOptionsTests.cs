using Sudoku.Web.Server.Components;

namespace UnitTests.Web.Components;

public class DifficultyLevelOptionsTests : TestContext
{
	[Fact]
	public void RenderComponent_RendersCorrectly()
	{
		// Arrange

        // Act
        var renderedContent = RenderComponent<DifficultyLevelOptions>();

        // Assert
		renderedContent.MarkupMatches("<div class=\"difficulty-level\"><div class=\"form-check form-check-inline\"><input class=\"form-check-input\" type=\"radio\" name=\"levelOptions\" id=\"levelEasy\" value=\"Easy\"  checked=\"\"><label class=\"form-check-label\" for=\"levelEasy\">Easy</label></div><div class=\"form-check form-check-inline\"><input class=\"form-check-input\" type=\"radio\" name=\"levelOptions\" id=\"levelMedium\" value=\"Medium\" ><label class=\"form-check-label\" for=\"levelMedium\">Medium</label></div><div class=\"form-check form-check-inline\"><input class=\"form-check-input\" type=\"radio\" name=\"levelOptions\" id=\"levelHard\" value=\"Hard\" ><label class=\"form-check-label\" for=\"levelHard\">Hard</label></div></div>");
	}
}