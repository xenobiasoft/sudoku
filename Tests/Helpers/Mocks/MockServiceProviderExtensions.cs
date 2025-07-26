using Sudoku.Domain.Events;
using XenobiaSoft.Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Helpers.Mocks;

public static class MockServiceProviderExtensions
{
    public static void SetupFaultyHandlers<TDomainEvent, TExceptionType>(this Mock<IServiceProvider> mock, TDomainEvent domainEvent) where TDomainEvent : DomainEvent where TExceptionType : Exception, new()
    {
        var mockHandler = new Mock<IDomainEventHandler<TDomainEvent>>();
        mockHandler
            .Setup(x => x.HandleAsync(domainEvent))
            .ThrowsAsync(new TExceptionType());

        var handlers = new[] { mockHandler.Object };
        mock.SetupGetService(typeof(IEnumerable<IDomainEventHandler<TDomainEvent>>), handlers);
    }

    public static void SetupGetService(this Mock<IServiceProvider> mock, Type serviceType, object? serviceInstance)
    {
        mock
            .Setup(sp => sp.GetService(serviceType))
            .Returns(serviceInstance);
    }
}