using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Mocks;

public static class MockServiceProviderExtensions
{
    extension(Mock<IServiceProvider> mock)
    {
        public void SetupFaultyHandlers<TDomainEvent, TExceptionType>(TDomainEvent domainEvent) where TDomainEvent : DomainEvent where TExceptionType : Exception, new()
        {
            var mockHandler = new Mock<IDomainEventHandler<TDomainEvent>>();
            mockHandler
                .Setup(x => x.HandleAsync(domainEvent))
                .ThrowsAsync(new TExceptionType());

            var handlers = new[] { mockHandler.Object };
            mock.SetupGetService(typeof(IEnumerable<IDomainEventHandler<TDomainEvent>>), handlers);
        }

        public void SetupGetService(Type serviceType, object? serviceInstance)
        {
            mock
                .Setup(sp => sp.GetService(serviceType))
                .Returns(serviceInstance);
        }
    }
}