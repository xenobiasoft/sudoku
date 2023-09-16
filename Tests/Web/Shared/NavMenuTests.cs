namespace UnitTests.Web.Shared;

public class NavMenuTests : TestContext
{
	[Fact]
	public void NavMenu_RendersCorrectly()
	{
		// Arrange
		var expected = "<div class=\"top-row ps-3 navbar navbar-dark\"><div class=\"container-fluid\"><a class=\"navbar-brand\" href=\"\"><img alt=\"XenobiaSoft Sudoku\" src=\"images/logo.png\"></a><button title=\"Navigation menu\" class=\"navbar-toggler\" ><span class=\"navbar-toggler-icon\"></span></button></div></div><div class=\"collapse nav-scrollable\" ><nav class=\"flex-column\"><div class=\"nav-item px-3\"><a href=\"/game\" class=\"nav-link\"><i class=\"fa-solid fa-play\"></i>&nbsp;New Game</a></div></nav></div>";

		// Act
		var content = RenderComponent<NavMenu>();

		// Assert
		content.MarkupMatches(expected);
	}
}