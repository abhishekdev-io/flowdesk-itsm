namespace FlowDesk.Application.Features.Tickets.TicketComments.RestoreComment;

public class RestoreCommentCommandValidator : AbstractValidator<RestoreCommentCommand>
{
    public RestoreCommentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");

        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required.");   

        RuleFor(x => x.CommentId)
            .NotEmpty()
            .WithMessage("Comment ID is required."); 
    }
}
