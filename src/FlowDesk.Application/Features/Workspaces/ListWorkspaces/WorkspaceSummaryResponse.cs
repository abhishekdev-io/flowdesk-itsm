namespace FlowDesk.Application.Features.Workspaces.ListWorkspaces;

public sealed record WorkspaceSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt
);

