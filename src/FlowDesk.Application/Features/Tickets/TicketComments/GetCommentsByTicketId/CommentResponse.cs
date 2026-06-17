using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Tickets.TicketComments.GetCommentsByTicketId;

public record CommentResponse(
    Guid Id,
    string Description,
    CommentAuthorDto Author,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsEdited
);
