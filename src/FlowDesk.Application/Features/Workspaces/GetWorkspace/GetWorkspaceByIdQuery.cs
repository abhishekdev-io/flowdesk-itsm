using MediatR;

namespace FlowDesk.Application.Features.Workspaces.GetWorkspace;

public record GetWorkspaceByIdQuery(
    Guid WorkspaceId
) : IRequest<WorkspaceResponse>;
