using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.DeleteWorkspace;

internal sealed class DeleteWorkspaceCommandValidation : AbstractValidator<DeleteWorkspaceCommand>
{
    public DeleteWorkspaceCommandValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
