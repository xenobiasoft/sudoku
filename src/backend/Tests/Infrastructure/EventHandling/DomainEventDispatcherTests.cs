using DepenMock.XUnit;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.EventHandling;
using UnitTests.Helpers.Builders;
using UnitTests.Mocks;

namespace UnitTests.Infrastructure.EventHandling;

public class DomainEventDispatcherTests : BaseTestByAbstraction<DomainEventDispatcher, IDomainEventDispatcher>
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public DomainEventDispatcherTests()
    {
        _mockServiceProvider = Container.ResolveMock<IServiceProvider>();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerThrows_RethrowsException()
    {
        // Arrange
        var domainEvent = Container.Create<GameCreatedEvent>();
        _mockServiceProvider.SetupFaultyHandlers<GameCreatedEvent, InvalidOperationException>(domainEvent);
        var sut = ResolveSut();

        // Act
        var dispatchAsync = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await dispatchAsync.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DispatchAsync_WithDifferentEventTypes_CallsCorrectHandlers()
    {
        // Arrange
        var domainEvent = Container.Create<MoveMadeEvent>();
        var mockHandler = new Mock<IDomainEventHandler<MoveMadeEvent>>();
        var handlers = new[] { mockHandler.Object };
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<MoveMadeEvent>>), handlers);

        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvent);

        // Assert
        mockHandler.Verify(x => x.HandleAsync(domainEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithEmptyEventCollection_DoesNotThrow()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var dispatchAsync = async () => await sut.DispatchAsync([]);

        // Assert
        await dispatchAsync.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DispatchAsync_WithMixedSuccessAndFailureHandlers_ThrowsFirstException()
    {
        // Arrange
        var domainEvent = Container.Create<GameCreatedEvent>();
        var mockHandler1 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var mockHandler2 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var expectedException = new InvalidOperationException("First handler error");
        
        mockHandler1
            .Setup(x => x.HandleAsync(domainEvent))
            .ThrowsAsync(expectedException);
        var handlers = new[] { mockHandler1.Object, mockHandler2.Object };
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>), handlers);
        var sut = ResolveSut();

        // Act
        var dispatchAsync = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await dispatchAsync.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("First handler error");
        
        // Verify first handler was called
        mockHandler1.Verify(x => x.HandleAsync(domainEvent), Times.Once);
        // Verify second handler was not called due to exception
        mockHandler2.Verify(x => x.HandleAsync(domainEvent), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleEvents_CallsHandlersForEachEvent()
    {
        // Arrange
        var gameCreatedEvent = Container.Create<GameCreatedEvent>();
        var gameStartedEvent = Container.Create<GameStartedEvent>();
        var domainEvents = new DomainEvent[] { gameCreatedEvent, gameStartedEvent };
        var mockCreatedHandler = Container.ResolveMock<IDomainEventHandler<GameCreatedEvent>>();
        var mockStartedHandler = Container.ResolveMock<IDomainEventHandler<GameStartedEvent>>();
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>), new[] { mockCreatedHandler.Object });
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameStartedEvent>>), new[] { mockStartedHandler.Object });
        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvents);

        // Assert
        mockCreatedHandler.Verify(x => x.HandleAsync(gameCreatedEvent), Times.Once);
        mockStartedHandler.Verify(x => x.HandleAsync(gameStartedEvent), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_WithMultipleHandlers_CallsAllHandlers()
    {
        // Arrange
        var domainEvent = Container.Create<GameCreatedEvent>();
        var mockHandler1 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var mockHandler2 = new Mock<IDomainEventHandler<GameCreatedEvent>>();
        var handlers = new[] { mockHandler1.Object, mockHandler2.Object };
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>), handlers);
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
        var domainEvent = Container.Create<GameCreatedEvent>();
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>), Array.Empty<IDomainEventHandler<GameCreatedEvent>>());
        var sut = ResolveSut();

        // Act
        var dispatchAsync = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await dispatchAsync.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DispatchAsync_WithServiceProviderThrows_RethrowsException()
    {
        // Arrange
        var domainEvent = Container.Create<GameCreatedEvent>();
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>)))
            .Throws(new InvalidOperationException());
        var sut = ResolveSut();

        // Act
        var dispatchAsync = async () => await sut.DispatchAsync(domainEvent);

        // Assert
        await dispatchAsync.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DispatchAsync_WithSingleEvent_CallsCorrectHandler()
    {
        // Arrange
        var domainEvent = Container.Create<GameCreatedEvent>();
        var mockHandler = Container.ResolveMock<IDomainEventHandler<GameCreatedEvent>>();
        var handlers = new[] { mockHandler.Object };
        _mockServiceProvider.SetupGetService(typeof(IEnumerable<IDomainEventHandler<GameCreatedEvent>>), handlers);
        var sut = ResolveSut();

        // Act
        await sut.DispatchAsync(domainEvent);

        // Assert
        mockHandler.Verify(x => x.HandleAsync(domainEvent), Times.Once);
    }
}