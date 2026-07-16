using FlowDesk.Application.Common.Dto;
using FlowDesk.Application.Interfaces;
using MediatR;

namespace FlowDesk.Application.Features.Workspaces.WorkspaceMember.GetMemberById;

public record GetMemberByIdQuery(
    Guid UserId,
    Guid WorkspaceId
) : IRequest<WorkspaceMemberDto>,IWorkspaceRequest;
