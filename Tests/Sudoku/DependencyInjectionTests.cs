using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Helpers;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Sudoku;

public class DependencyInjectionTests
{
	[Fact]
    public void DIContainer_WhenServicesRegisteredSuccessfully_CanResolveServices()
    {
        // Arrange
        var configMgr = new ConfigurationManager();
        configMgr
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var services = new ServiceCollection();
        services
            .RegisterBlazorGameServices(configMgr)
            .RegisterGameServices(configMgr);
        var factory = new TestServiceProviderFactory();
        var serviceProvider = factory.CreateServiceProvider(factory.CreateBuilder(services));

        // Act
        var gameInstance = serviceProvider.GetService<ISudokuGame>();

        // Assert
        gameInstance.Should().NotBeNull();
    }
}