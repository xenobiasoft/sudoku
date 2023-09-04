namespace UnitTests.Web.Shared;

public class NavMenuTests : TestContext
{
	[Fact]
	public void NavMenu_RendersCorrectly()
	{
		// Arrange
		var expected = "<div class=\"top-row ps-3 navbar navbar-dark\"><div class=\"container-fluid\"><a class=\"navbar-brand\" href>XenobiaSoft Sudoku</a><button title=\"Navigation menu\" class=\"navbar-toggler\" blazor:onclick=\"1\" b-jlhp5ntr5c><span class=\"navbar-toggler-icon\" b-jlhp5ntr5c></span></button></div></div>\r\n\r\n<div class=\"collapse\" blazor:onclick=\"2\" b-jlhp5ntr5c><nav class=\"flex-column\" b-jlhp5ntr5c><div class=\"nav-item px-3\" b-jlhp5ntr5c><a href=\"\" class=\"nav-link active\"><span class=\"oi oi-home\" aria-hidden=\"true\"></span>Home</a></div></nav></div>";

		// Act
		var content = RenderComponent<NavMenu>();

		// Assert
		content.MarkupMatches(expected);
	}
}