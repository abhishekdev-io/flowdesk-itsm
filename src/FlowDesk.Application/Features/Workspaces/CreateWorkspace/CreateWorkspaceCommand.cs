using MediatR;

namespace FlowDesk.Application.Features.Workspaces.CreateWorkspace;

public record CreateWorkspaceCommand(
    string Name,
    string Description
) : IRequest<Guid>;
