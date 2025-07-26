using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Domain.Events;

namespace XenobiaSoft.Sudoku.Infrastructure.EventHandling;

public class DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(DomainEvent domainEvent)
    {
        try
        {
            logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method == null) continue;

                var task = (Task)method.Invoke(handler, [domainEvent])!;
                await task;
            }

            logger.LogInformation("Successfully dispatched domain event: {EventType}", domainEvent.GetType().Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error dispatching domain event: {EventType}", domainEvent.GetType().Name);
            throw;
        }
    }

    public async Task DispatchAsync(IEnumerable<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent);
        }
    }
}