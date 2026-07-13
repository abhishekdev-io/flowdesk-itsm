using FlowDesk.Application.Common.Permission;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.Tickets.GetSlaDashboard;

[RequirePermission(Permissions.CanManageWorkspace)]
public sealed record GetSlaDashBoardQuery(
    Guid WorkspaceId
) : IRequest<SlaDashboardDto>,IWorkspaceRequest;


public record SlaDashboardDto
(
    int Total,
    int Breached,
    int Critical,
    int AtRisk,
    int OnTrack,
    int NoSla,
    double CompliancePercent
);

 