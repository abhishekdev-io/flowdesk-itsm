namespace FlowDesk.Application.Features.Tickets.CloseTicket;

public class CloseTicketCommandValidator : AbstractValidator<CloseTicketCommand>
{
    public CloseTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");  

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500)
            .When(x => x.Reason is not null);  
    }
}
