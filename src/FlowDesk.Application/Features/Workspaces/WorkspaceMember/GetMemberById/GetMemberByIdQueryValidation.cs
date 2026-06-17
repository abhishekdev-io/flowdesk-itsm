using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.GetMemberById;

internal sealed class GetMemberByIdQueryValidation : AbstractValidator<GetMemberByIdQuery>
{
    public GetMemberByIdQueryValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId is required.");
    }
}
