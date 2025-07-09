using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.Services;

public class GameServiceTests : BaseTestByAbstraction<GameService, IGameService>
{
	[Fact]
	public async Task Fails()
	{
		// Arrange


		// Act

		// Assert
		Assert.Fail("Not Implemented");
	}
}