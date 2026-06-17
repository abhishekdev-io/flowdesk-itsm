namespace FlowDesk.Application.Features.Tickets.UnAssignTicket;

internal sealed class UnAssignTicketCommandValidator : AbstractValidator<UnAssignTicketCommand>
{
    public UnAssignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
