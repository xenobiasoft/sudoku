using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Storage.Azure;
using XenobiaSoft.Sudoku.Storage.Azure.GameState;

namespace UnitTests.Storage.Azure;

public class DependencyInjectionTests
{
    private IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureStorage:ConnectionString"] = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=dGVzdA==;EndpointSuffix=core.windows.net"
            })
            .Build();

        var services = new ServiceCollection();
        
        // Add required framework services
        services.AddLogging();
        
        // Add Azure Storage services using the extension method
        services.AddAzureStorage(configuration);

        var factory = new TestServiceProviderFactory();
        return factory.CreateServiceProvider(factory.CreateBuilder(services));
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_CanResolveStorageService()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var storageService = serviceProvider.GetService<IStorageService>();

        // Assert
        storageService.Should().NotBeNull();
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_CanResolvePersistentGameStorage()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var persistentGameStorage = serviceProvider.GetService<IPersistentGameStateStorage>();

        // Assert
        persistentGameStorage.Should().NotBeNull();
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_CanResolveAzureBlobStorage()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var azureBlobStorage = serviceProvider.GetService<AzureBlobGameStateStorage>();

        // Assert
        azureBlobStorage.Should().NotBeNull();
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_VerifyServiceLifetimes()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act & Assert - Verify scoped services return the same instance within scope
        using (var scope = serviceProvider.CreateScope())
        {
            var storageService1 = scope.ServiceProvider.GetService<IStorageService>();
            var storageService2 = scope.ServiceProvider.GetService<IStorageService>();
            
            storageService1.Should().BeSameAs(storageService2, "Scoped services should return the same instance within a scope");
        }

        // Verify different scopes get different instances
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        
        var storageServiceScope1 = scope1.ServiceProvider.GetService<IStorageService>();
        var storageServiceScope2 = scope2.ServiceProvider.GetService<IStorageService>();
        
        storageServiceScope1.Should().NotBeSameAs(storageServiceScope2, "Different scopes should get different instances");
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_AllRequiredServicesAreRegistered()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var requiredServiceTypes = new[]
        {
            typeof(IStorageService),
            typeof(IPersistentGameStateStorage),
            typeof(AzureBlobGameStateStorage)
        };

        // Act & Assert
        foreach (var serviceType in requiredServiceTypes)
        {
            var service = serviceProvider.GetService(serviceType);
            service.Should().NotBeNull($"Service {serviceType.Name} should be registered");
        }
    }

    [Fact]
    public void AzureStorage_WhenMissingConnectionString_ThrowsInformativeException()
    {
        // Arrange
        var emptyConfiguration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var act = () => services.AddAzureStorage(emptyConfiguration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Azure Storage connection string not found*");
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_PersistentStorageUsesDecorator()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var persistentStorage = serviceProvider.GetService<IPersistentGameStateStorage>();
        var azureBlobStorage = serviceProvider.GetService<AzureBlobGameStateStorage>();

        // Assert
        persistentStorage.Should().NotBeNull();
        azureBlobStorage.Should().NotBeNull();
        
        // The persistent storage should be the decorator, not the raw Azure blob storage
        persistentStorage.Should().NotBeSameAs(azureBlobStorage, "IPersistentGameStateStorage should be a decorator around AzureBlobGameStateStorage");
    }

    [Fact]
    public void AzureStorage_WhenServicesRegistered_CanCreateComplexServiceGraphs()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act - Resolve services that have complex dependency graphs
        var persistentStorage = serviceProvider.GetService<IPersistentGameStateStorage>();
        var storageService = serviceProvider.GetService<IStorageService>();

        // Assert
        persistentStorage.Should().NotBeNull();
        storageService.Should().NotBeNull();
    }
}