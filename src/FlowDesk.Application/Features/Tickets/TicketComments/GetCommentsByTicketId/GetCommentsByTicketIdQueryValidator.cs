namespace FlowDesk.Application.Features.Tickets.TicketComments.GetCommentsByTicketId;

internal sealed class GetCommentsByTicketIdQueryValidator : AbstractValidator<GetCommentsByTicketIdQuery>
{
    public GetCommentsByTicketIdQueryValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");    

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);    
    }
}
