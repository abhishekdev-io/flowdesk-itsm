namespace FlowDesk.Application.Features.Tickets.GetTicket;

internal sealed class GetTicketByIdQueryValidation : AbstractValidator<GetTicketByIdQuery>
{
    public GetTicketByIdQueryValidation()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
            
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
