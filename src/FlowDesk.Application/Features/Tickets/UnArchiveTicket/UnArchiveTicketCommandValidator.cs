namespace FlowDesk.Application.Features.Tickets.UnArchiveTicket;

internal sealed class UnArchiveTicketCommandValidator : AbstractValidator<UnArchiveTicketCommand>
{
    public UnArchiveTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");    
    }
}
