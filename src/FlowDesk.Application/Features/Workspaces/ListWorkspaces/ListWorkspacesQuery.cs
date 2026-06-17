using MediatR;

namespace FlowDesk.Application.Features.Workspaces.ListWorkspaces;

public record ListWorkspacesQuery() : IRequest<IEnumerable<WorkspaceSummaryResponse>>;
