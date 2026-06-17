using FlowDesk.Application.Interface;
using MediatR;

namespace FlowDesk.Application.Features.SystemAdmin.ListDeletedWorkspaces;

public record ListDeletedWorkspacesQuery() : IRequest<IEnumerable<DeletedWorkspaceSummaryResponse>>,ISystemAdminRequest;
