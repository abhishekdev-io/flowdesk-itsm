namespace FlowDesk.Application.Features.Tickets.GetSlaDashboard;

internal sealed class GetSlaDashboardQueryValidator : AbstractValidator<GetSlaDashBoardQuery>
{
    public GetSlaDashboardQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
