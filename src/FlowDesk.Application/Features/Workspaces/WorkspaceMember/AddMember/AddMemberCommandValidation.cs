using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.AddMember;

internal sealed class AddMemberCommandValidation : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Role is invalid.");
    }
}
