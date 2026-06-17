namespace FlowDesk.Application.Features.Tickets.ListTickets;

internal sealed class ListTicketsQueryValidator : AbstractValidator<ListTicketsQuery>
{
    public ListTicketsQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
        
        RuleFor(x => x.Filters)
            .SetValidator(new TicketsFilterParamsValidator()!)
            .When(x => x.Filters is not null);
    }
}
