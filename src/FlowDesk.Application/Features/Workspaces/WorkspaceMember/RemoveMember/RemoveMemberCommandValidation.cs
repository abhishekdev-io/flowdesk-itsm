using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.RemoveMember;

internal sealed class RemoveMemberCommandValidation : AbstractValidator<RemoveMemberCommand>
{
    public RemoveMemberCommandValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId is required.");
    }
}
