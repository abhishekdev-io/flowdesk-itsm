using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Tickets;

namespace FlowDesk.Application.Features.Tickets.GetTicketActivities;

public sealed class GetTicketActivitiesQueryHandler(
    IApplicationDbContext context,
    ITicketRepository ticketRepository
    ) : IRequestHandler<GetTicketActivitiesQuery, IReadOnlyCollection<TicketActivityResponse>>
{
    public async Task<IReadOnlyCollection<TicketActivityResponse>> Handle(GetTicketActivitiesQuery request, CancellationToken cancellationToken)
    {
        var exist = await ticketRepository.ExistAsync(
            request.TicketId,
            request.WorkspaceId,
            cancellationToken);

        if(!exist)
            throw new NotFoundException("Ticket",request.TicketId);  

        return await context.TicketActivities
            .Where(a => a.TicketId == request.TicketId)   
            .OrderBy(a => a.OccurredAt) 
            .Select(a => new TicketActivityResponse(
                a.Id,

                context.WorkspaceUsersView
                   .Where(wv => wv.UserId == a.PerformedBy)
                   .Select(u => new PerformedbyDto(
                        u.UserName,
                        u.Role,
                        u.AvatarUrl
                    )).FirstOrDefault()!,
                    
                a.OccurredAt,
                a.Action.ToString(),
                a.Description!    
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);  
    }
}
