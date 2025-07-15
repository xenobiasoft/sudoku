using DepenMock.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using XenobiaSoft.Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Infrastructure.EventHandling;

public class DomainEventDispatcherTests : BaseTestByAbstraction<DomainEventDispatcher, IDomainEventDispatcher>
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public DomainEventDispatcherTests()
    {
        _mockServiceProvider = Container.ResolveMock<IServiceProvider>();
    }

    [Fact]
    public async Task DispatchAsync_WithSingleEvent_CallsCorrectHandler()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var mockHandler = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        mockHandler.Setup(x => x.HandleAsync(domainEvent))
            .Returns(Task.CompletedTask);

        var handlers = new[] { mockHandler.Object };

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(handlers);

        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvent);

        // Assert
        mockHandler.Verify(x => x.HandleAsync(domainEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleHandlers_CallsAllHandlers()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var mockHandler1 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var mockHandler2 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        
        mockHandler1.Setup(x => x.HandleAsync(domainEvent))
            .Returns(Task.CompletedTask);
        mockHandler2.Setup(x => x.HandleAsync(domainEvent))
            .Returns(Task.CompletedTask);

        var handlers = new[] { mockHandler1.Object, mockHandler2.Object };

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(handlers);

        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvent);

        // Assert
        mockHandler1.Verify(x => x.HandleAsync(domainEvent), Times.Once);
        mockHandler2.Verify(x => x.HandleAsync(domainEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlers_DoesNotThrow()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var emptyHandlers = Array.Empty<IDomainEventHandler<GameCreatedEvent>>();

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(emptyHandlers);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var mockHandler = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var expectedException = new InvalidOperationException("Handler error");
        
        mockHandler.Setup(x => x.HandleAsync(domainEvent))
            .ThrowsAsync(expectedException);

        var handlers = new[] { mockHandler.Object };

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(handlers);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Handler error");
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleEvents_CallsHandlersForEachEvent()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var gameCreatedEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);
        var gameStartedEvent = new GameStartedEvent(gameId);
        var domainEvents = new DomainEvent[] { gameCreatedEvent, gameStartedEvent };

        var mockCreatedHandler = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var mockStartedHandler = new Mock<IDomainEventHandler<GameStartedEvent>>();
        
        mockCreatedHandler.Setup(x => x.HandleAsync(gameCreatedEvent))
            .Returns(Task.CompletedTask);
        mockStartedHandler.Setup(x => x.HandleAsync(gameStartedEvent))
            .Returns(Task.CompletedTask);

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(new[] { mockCreatedHandler.Object });
        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameStartedEvent>)))
            .Returns(new[] { mockStartedHandler.Object });

        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvents);

        // Assert
        mockCreatedHandler.Verify(x => x.HandleAsync(gameCreatedEvent), Times.Once);
        mockStartedHandler.Verify(x => x.HandleAsync(gameStartedEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithEmptyEventCollection_DoesNotThrow()
    {
        // Arrange
        var emptyEvents = Array.Empty<DomainEvent>();
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync(emptyEvents);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DispatchAsync_WithDifferentEventTypes_CallsCorrectHandlers()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var statistics = GameStatistics.Create();
        
        var moveMadeEvent = new MoveMadeEvent(gameId, 0, 0, 5, statistics);

        var mockHandler = new Mock<IDomainEventHandler<MoveMadeEvent>>();
        mockHandler.Setup(x => x.HandleAsync(moveMadeEvent))
            .Returns(Task.CompletedTask);

        var handlers = new[] { mockHandler.Object };

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<MoveMadeEvent>)))
            .Returns(handlers);

        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(moveMadeEvent);

        // Assert
        mockHandler.Verify(x => x.HandleAsync(moveMadeEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithServiceProviderThrows_RethrowsException()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var expectedException = new InvalidOperationException("Service provider error");
        
        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Throws(expectedException);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Service provider error");
    }

    [Fact]
    public async Task DispatchAsync_WithMixedSuccessAndFailureHandlers_ThrowsFirstException()
    {
        // Arrange
        var gameId = GameId.New();
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var domainEvent = new GameCreatedEvent(gameId, playerAlias, difficulty);

        var mockHandler1 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var mockHandler2 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var expectedException = new InvalidOperationException("First handler error");
        
        mockHandler1.Setup(x => x.HandleAsync(domainEvent))
            .ThrowsAsync(expectedException);
        mockHandler2.Setup(x => x.HandleAsync(domainEvent))
            .Returns(Task.CompletedTask);

        var handlers = new[] { mockHandler1.Object, mockHandler2.Object };

        _mockServiceProvider.Setup(x => x.GetServices(typeof(IDomainEventHandler<GameCreatedEvent>)))
            .Returns(handlers);

        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("First handler error");
        
        // Verify first handler was called
        mockHandler1.Verify(x => x.HandleAsync(domainEvent), Times.Once);
        // Verify second handler was not called due to exception
        mockHandler2.Verify(x => x.HandleAsync(domainEvent), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_WithNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync((DomainEvent)null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DispatchAsync_WithNullEventCollection_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.DispatchAsync((IEnumerable<DomainEvent>)null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}