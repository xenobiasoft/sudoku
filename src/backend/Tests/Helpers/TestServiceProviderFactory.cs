using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Helpers;

public class TestServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
	public IServiceCollection CreateBuilder(IServiceCollection services)
	{
		return services;
	}

	public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
	{
		var serviceProvider = containerBuilder.BuildServiceProvider();

		return serviceProvider;
	}
}
