namespace FlowDesk.Application.Features.Tickets.GetTicketActivities;

internal sealed class GetTicketActivitiesQueryValidator : AbstractValidator<GetTicketActivitiesQuery>
{
    public GetTicketActivitiesQueryValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
            
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
