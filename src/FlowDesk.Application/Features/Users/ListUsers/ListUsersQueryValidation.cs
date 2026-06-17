using FluentValidation;

namespace FlowDesk.Application.Features.Users.ListUsers;

public class ListUsersQueryValidation : AbstractValidator<ListUsersQuery>
{
    public ListUsersQueryValidation()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
    }
}
