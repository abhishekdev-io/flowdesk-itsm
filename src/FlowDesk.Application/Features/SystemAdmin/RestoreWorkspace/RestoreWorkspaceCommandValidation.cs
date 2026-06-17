using FluentValidation;

namespace FlowDesk.Application.Features.SystemAdmin.RestoreWorkspace;

internal sealed class RestoreWorkspaceCommandValidation : AbstractValidator<RestoreWorkspaceCommand>
{
    public RestoreWorkspaceCommandValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
