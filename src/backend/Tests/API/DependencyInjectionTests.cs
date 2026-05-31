using MediatR;
using Microsoft.AspNetCore.Hosting;
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
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var controller = new GamesController(mediator);

        // Assert
        controller.Should().NotBeNull();
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
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers();

        services.AddApiDefaults(invalidConfiguration, mockEnvironment.Object);

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var act = () => serviceProvider.GetRequiredService<IAzureStorageService>();

        act.Should().Throw<Exception>();
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
        var mediator = serviceProvider.GetService<IMediator>();

        // Assert
        mediator.Should().NotBeNull();
    }

    [Fact]
    public void ApiDefaults_WhenServicesRegistered_AllRequiredServicesAreRegistered()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var requiredServiceTypes = new[]
        {
            typeof(IMediator),
            typeof(IGameRepository),
            typeof(IPuzzleRepository),
            typeof(IPuzzleGenerator),
            typeof(IPuzzleSolver),
            typeof(IDomainEventDispatcher),
            typeof(IPuzzlePoolService),
            typeof(IPuzzleBlobStorage)
        };

        // Act & Assert
        foreach (var serviceType in requiredServiceTypes)
        {
            var service = serviceProvider.GetService(serviceType);
            service.Should().NotBeNull($"Service {serviceType.Name} should be registered");
        }
    }

    private IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureStorage:ConnectionString"] = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=dGVzdA==;EndpointSuffix=core.windows.net",
                ["AzureStorage:UseManagedIdentity"] = "false"
            })
            .Build();
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddControllers();

        services.AddApiDefaults(configuration, mockEnvironment.Object);

        services.AddTransient<GamesController>();

        var factory = new TestServiceProviderFactory();
        return factory.CreateServiceProvider(factory.CreateBuilder(services));
    }
}
