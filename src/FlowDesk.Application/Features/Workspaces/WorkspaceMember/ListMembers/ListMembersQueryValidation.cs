using FluentValidation;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.ListMembers;

internal sealed class ListMembersQueryValidation : AbstractValidator<ListMembersQuery>
{
    public ListMembersQueryValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
