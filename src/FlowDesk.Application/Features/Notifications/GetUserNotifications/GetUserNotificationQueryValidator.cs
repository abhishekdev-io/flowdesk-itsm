namespace FlowDesk.Application.Features.Notifications.GetUserNotifications;

internal sealed class GetUserNotificationQueryValidator : AbstractValidator<GetUserNotificationsQuery>
{
    public GetUserNotificationQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");    

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
