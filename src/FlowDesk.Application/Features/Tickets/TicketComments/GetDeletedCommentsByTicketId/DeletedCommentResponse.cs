using FlowDesk.Application.Common.Dto;

namespace FlowDesk.Application.Features.Tickets.TicketComments.GetDeletedCommentsByTicketId;

public record DeletedCommentResponse(
    Guid Id,
    string Description,
    CommentAuthorDto Author,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsEdited,
    DateTime? DeletedAt
);
