using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Features.SystemAdmin.CreateSystemAdmin;

public record CreateSystemAdminCommand(
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    string AvatarUrl
) : IRequest<Guid>, ISystemAdminRequest;
