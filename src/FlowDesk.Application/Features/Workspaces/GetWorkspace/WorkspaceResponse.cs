namespace FlowDesk.Application.Features.Workspaces.GetWorkspace;

public sealed record WorkspaceResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt
);
