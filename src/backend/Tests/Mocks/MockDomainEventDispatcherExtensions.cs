using Sudoku.Domain.Events;
using Sudoku.Infrastructure.EventHandling;

namespace UnitTests.Mocks;

public static class MockDomainEventDispatcherExtensions
{
    extension(Mock<IDomainEventDispatcher> mock)
    {
        public void SetupDispatchThrows(Exception ex)
        {
            mock
                .Setup(x => x.DispatchAsync(It.IsAny<IEnumerable<DomainEvent>>()))
                .ThrowsAsync(ex);
        }

        public void VerifyDispatched<TEvent>(Func<Times> times) where TEvent : DomainEvent
        {
            mock.Verify(x => x.DispatchAsync(It.Is<IEnumerable<DomainEvent>>(events => events.OfType<TEvent>().Any())), times);
        }

        public void VerifyDispatchedEventCount(int expectedCount, Func<Times> times)
        {
            mock.Verify(x => x.DispatchAsync(It.Is<IEnumerable<DomainEvent>>(events => events.Count() == expectedCount)), times);
        }
    }
}
