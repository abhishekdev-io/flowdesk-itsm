namespace FlowDesk.Application.Features.Tickets.ChangePriority;

public class ChangePriorityCommandValidator : AbstractValidator<ChangePriorityCommand>
{
    public ChangePriorityCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
        
        RuleFor(x => x.NewPriority)
            .IsInEnum()
            .WithMessage("Invalid priority value.");
    }
}
