namespace FlowDesk.Application.Features.Notifications.DeleteNotification;

internal sealed class DeleteMultipleNotificationCommandValidator : AbstractValidator<DeleteMultipleNotificationsCommand>
{
    public DeleteMultipleNotificationCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");
        
        RuleFor(x => x.NotificationIds)
            .NotEmpty()
            .WithMessage("At least one notification ID is required.")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Notification IDs must not contain empty GUIDs.");
    }
}
