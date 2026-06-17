using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.CreateWorkspace;

internal sealed class CreateWorkspaceCommandValidation : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workspace name is required.")
            .MaximumLength(100).WithMessage("Asset name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Workspace description is required.")
            .MaximumLength(800).WithMessage("Workspace description cannot exceed 800 characters.");    
    }
}
