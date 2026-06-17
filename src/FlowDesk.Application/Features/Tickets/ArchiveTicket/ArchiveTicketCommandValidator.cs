namespace FlowDesk.Application.Features.Tickets.ArchiveTicket;

public class ArchiveTicketCommandValidator : AbstractValidator<ArchiveTicketCommand>
{
    public ArchiveTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");    
    }
}
