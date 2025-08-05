using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Api;
using Sudoku.Api.Controllers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers;

namespace UnitTests.API;

public class DependencyInjectionTests
{
    private IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureStorage:ConnectionString"] = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=dGVzdA==;EndpointSuffix=core.windows.net",
                ["AzureStorage:UseManagedIdentity"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        
        // Add required framework services
        services.AddLogging();
        services.AddControllers();
        
        // Add API services using the extension method
        services.AddApiDefaults(configuration);
        
        // Register controllers manually for testing
        services.AddTransient<GamesController>();
        services.AddTransient<PlayersController>();

        var factory = new TestServiceProviderFactory();
        return factory.CreateServiceProvider(factory.CreateBuilder(services));
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveGamesController()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var controller = serviceProvider.GetService<GamesController>();

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolvePlayersController()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var controller = serviceProvider.GetService<PlayersController>();

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveApplicationServices()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var gameApplicationService = serviceProvider.GetService<IGameApplicationService>();
        var playerApplicationService = serviceProvider.GetService<IPlayerApplicationService>();

        // Assert
        gameApplicationService.Should().NotBeNull();
        playerApplicationService.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveInfrastructureServices()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var gameRepository = serviceProvider.GetService<IGameRepository>();
        var puzzleRepository = serviceProvider.GetService<IPuzzleRepository>();
        var eventDispatcher = serviceProvider.GetService<IDomainEventDispatcher>();

        // Assert
        gameRepository.Should().NotBeNull();
        puzzleRepository.Should().NotBeNull();
        eventDispatcher.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveDomainEventHandlers()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var gameCreatedHandler = serviceProvider.GetService<IDomainEventHandler<GameCreatedEvent>>();
        var moveMadeHandler = serviceProvider.GetService<IDomainEventHandler<MoveMadeEvent>>();
        var gameCompletedHandler = serviceProvider.GetService<IDomainEventHandler<GameCompletedEvent>>();

        // Assert
        gameCreatedHandler.Should().NotBeNull();
        moveMadeHandler.Should().NotBeNull();
        gameCompletedHandler.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolvePuzzleServices()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var puzzleGenerator = serviceProvider.GetService<IPuzzleGenerator>();
        var puzzleSolver = serviceProvider.GetService<IPuzzleSolver>();

        // Assert
        puzzleGenerator.Should().NotBeNull();
        puzzleSolver.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanCreateControllerWithDependencies()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var gameApplicationService = serviceProvider.GetRequiredService<IGameApplicationService>();

        // Act
        var controller = new GamesController(gameApplicationService);

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_VerifyServiceLifetimes()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act & Assert - Verify scoped services return the same instance within scope
        using (var scope = serviceProvider.CreateScope())
        {
            var gameService1 = scope.ServiceProvider.GetService<IGameApplicationService>();
            var gameService2 = scope.ServiceProvider.GetService<IGameApplicationService>();
            
            gameService1.Should().BeSameAs(gameService2, "Scoped services should return the same instance within a scope");
        }

        // Verify different scopes get different instances
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        
        var gameServiceScope1 = scope1.ServiceProvider.GetService<IGameApplicationService>();
        var gameServiceScope2 = scope2.ServiceProvider.GetService<IGameApplicationService>();
        
        gameServiceScope1.Should().NotBeSameAs(gameServiceScope2, "Different scopes should get different instances");
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_AllRequiredServicesAreRegistered()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var requiredServiceTypes = new[]
        {
            typeof(IGameApplicationService),
            typeof(IPlayerApplicationService),
            typeof(IGameRepository),
            typeof(IPuzzleRepository),
            typeof(IPuzzleGenerator),
            typeof(IPuzzleSolver),
            typeof(IDomainEventDispatcher)
        };

        // Act & Assert
        foreach (var serviceType in requiredServiceTypes)
        {
            var service = serviceProvider.GetService(serviceType);
            service.Should().NotBeNull($"Service {serviceType.Name} should be registered");
        }
    }

    [Fact]
    public void ApiDefaults_WhenInvalidAzureConfiguration_ExceptionThrownDuringServiceCreation()
    {
        // Arrange
        var invalidConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureStorage:ConnectionString"] = "invalid-connection-string"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers();
        
        // This should work fine
        services.AddApiDefaults(invalidConfiguration);
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert - Exception should be thrown when trying to use Azure services
        var act = () => serviceProvider.GetRequiredService<IAzureStorageService>();
        
        act.Should().Throw<Exception>(); // Some form of exception will be thrown when invalid configuration is used
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveAzureStorageService()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var azureStorageService = serviceProvider.GetService<IAzureStorageService>();

        // Assert
        azureStorageService.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveAllEventHandlers()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var gameCreatedHandlers = serviceProvider.GetServices<IDomainEventHandler<GameCreatedEvent>>();
        var moveMadeHandlers = serviceProvider.GetServices<IDomainEventHandler<MoveMadeEvent>>();
        var gameCompletedHandlers = serviceProvider.GetServices<IDomainEventHandler<GameCompletedEvent>>();

        // Assert
        gameCreatedHandlers.Should().NotBeEmpty();
        moveMadeHandlers.Should().NotBeEmpty(); 
        gameCompletedHandlers.Should().NotBeEmpty();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_CanResolveMediatRHandlers()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();

        // Act
        var mediator = serviceProvider.GetService<MediatR.IMediator>();

        // Assert
        mediator.Should().NotBeNull();
    }
}