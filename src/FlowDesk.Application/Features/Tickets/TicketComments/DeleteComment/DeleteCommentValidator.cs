namespace FlowDesk.Application.Features.Tickets.TicketComments.DeleteComment;

internal sealed class DeleteCommentValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentValidator()
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
