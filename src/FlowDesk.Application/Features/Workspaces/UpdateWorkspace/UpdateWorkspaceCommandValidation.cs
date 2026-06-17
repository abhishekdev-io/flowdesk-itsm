using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.UpdateWorkspace;

public class UpdateWorkspaceCommandValidation : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workspace name is required.")
            .MaximumLength(100).WithMessage("Asset name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Workspace description is required.")
            .MaximumLength(800).WithMessage("Workspace description cannot exceed 800 characters."); 
    }
}
