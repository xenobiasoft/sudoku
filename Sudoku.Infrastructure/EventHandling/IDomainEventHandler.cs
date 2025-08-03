using Sudoku.Domain.Events;

namespace Sudoku.Infrastructure.EventHandling;

public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent);
}