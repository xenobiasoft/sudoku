using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent);
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents);
}