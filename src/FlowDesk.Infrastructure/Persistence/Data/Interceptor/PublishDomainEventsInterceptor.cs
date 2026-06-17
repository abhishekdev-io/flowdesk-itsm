using FlowDesk.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FlowDesk.Infrastructure.Persistence.Data.Interceptor;

public class PublishDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
private readonly IMediator _mediator = mediator;
    
    private readonly List<IDomainEvent> _capturedEvents = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            var aggregateRoots = eventData.Context.ChangeTracker.Entries<AggregateRoot<Guid>>()
                .Select(e => e.Entity);

            foreach (var aggregate in aggregateRoots)
            {
                _capturedEvents.AddRange(aggregate.PopDomainEvents());
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, 
        int result, 
        CancellationToken cancellationToken = default)
    {
        if (_capturedEvents.Count > 0)
        {
            var eventsToPublish = _capturedEvents.ToList();
            _capturedEvents.Clear();

            foreach (var domainEvent in eventsToPublish)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
