namespace FlowDesk.Application.Features.SystemAdmin.ListDeletedWorkspaces;

public record DeletedWorkspaceSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime? DeletedAt
);