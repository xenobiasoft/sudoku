namespace UnitTests.Web.Shared;

public class NavMenuTests : TestContext
{
	[Fact]
	public void NavMenu_RendersCorrectly()
	{
		// Arrange
		var expected = "<div class=\"top-row ps-3 navbar navbar-dark\" ><div class=\"container-fluid\" ><a class=\"navbar-brand\" href=\"\" ><img alt=\"XenobiaSoft Sudoku\" src=\"images/logo.png\" ></a><button title=\"Navigation menu\" class=\"navbar-toggler\" ><span class=\"navbar-toggler-icon\" ></span></button></div>\r\n</div>\r\n<div class=\"collapse\" ><nav class=\"flex-column\" ><div class=\"nav-item px-3\" ><a href=\"\" class=\"nav-link active\"><i class=\"fa-solid fa-house\" ></i>Home</a></div></nav></div>";

		// Act
		var content = RenderComponent<NavMenu>();

		// Assert
		content.MarkupMatches(expected);
	}
}