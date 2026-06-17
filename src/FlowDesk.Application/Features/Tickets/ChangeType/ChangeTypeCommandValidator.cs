namespace FlowDesk.Application.Features.Tickets.ChangeType;

public class ChangeTypeCommandValidator : AbstractValidator<ChangeTypeCommand>
{
    public ChangeTypeCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
        
        RuleFor(x => x.NewType)
            .IsInEnum()
            .WithMessage("Invalid ticket type value.");
    }
}
