using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests;

public class DependencyInjectionTests
{
	[Fact]
	public void DIContainer_WhenServicesRegisteredSuccessfully_CanResolveServices()
	{
		// Arrange
		var services = new ServiceCollection();
		services.RegisterGameServices();
		var factory = new TestServiceProviderFactory();
		var serviceProvider = factory.CreateServiceProvider(factory.CreateBuilder(services));

		// Act
		var gameInstance = serviceProvider.GetService<ISudokuGame>();

		// Assert
		gameInstance.Should().NotBeNull();
	}
}