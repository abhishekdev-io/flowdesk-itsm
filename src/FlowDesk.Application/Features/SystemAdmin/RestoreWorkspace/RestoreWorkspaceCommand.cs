using FlowDesk.Application.Interface;
using MediatR;

namespace FlowDesk.Application.Features.SystemAdmin.RestoreWorkspace;

public record RestoreWorkspaceCommand(
    Guid WorkspaceId
) : IRequest<Unit>,ISystemAdminRequest;
