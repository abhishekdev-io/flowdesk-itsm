using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ChangeMemberRole;

internal sealed class ChangeMemberRoleCommandValidation : AbstractValidator<ChangeMemberRoleCommand>
{
    public ChangeMemberRoleCommandValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Role is invalid.");
    }
}
