using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.GetWorkspace;

internal sealed class GetWorkspaceByIdQueryValidation : AbstractValidator<GetWorkspaceByIdQuery>
{
    public GetWorkspaceByIdQueryValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
