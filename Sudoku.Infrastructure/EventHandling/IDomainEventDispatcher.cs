using Sudoku.Domain.Events;

namespace XenobiaSoft.Sudoku.Infrastructure.EventHandling;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent);
    Task DispatchAsync(IEnumerable<DomainEvent> domainEvents);
}