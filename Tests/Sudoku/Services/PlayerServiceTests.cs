using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.Services;

public class PlayerServiceTests : BaseTestByAbstraction<PlayerService, IPlayerService>
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