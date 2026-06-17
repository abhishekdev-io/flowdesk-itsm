namespace FlowDesk.Application.Features.Tickets.ListTickets;

internal sealed class TicketsFilterParamsValidator : AbstractValidator<TicketFilterParams>
{
    private static readonly HashSet<string> AllowedSortFields = new(
    ["createdat", "priority", "status", "type"],
    StringComparer.OrdinalIgnoreCase
    );

    public TicketsFilterParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        When(x => x.SortBy is not null, () =>
        {
            RuleFor(x => x.SortBy!)
                .Must(s => AllowedSortFields.Contains(s))
                .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
        });

        When(x => x.Search is not null, () =>
        {
            RuleFor(x => x.Search!)
                .NotEmpty()
                .WithMessage("Search must not be empty or whitespace.")
                .MaximumLength(200);
        });   

        When(x => x.AssignedToUserId is not null, () =>
        {
            RuleFor(x => x.AssignedToUserId)
                .NotEmpty()
                .WithMessage("AssignedToUserId must be a valid non-empty GUID.");
        }); 

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue); 

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);       
    }
}
 