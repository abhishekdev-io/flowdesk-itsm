using FlowDesk.Application.Interfaces;

namespace FlowDesk.Application.Features.SystemAdmin.CreateSystemAdmin;

public class CreateSystemAdminCommandHandler(
    IAuthService authService
    ) : IRequestHandler<CreateSystemAdminCommand,Guid>
{
    public async Task<Guid> Handle(CreateSystemAdminCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterSystemAdminAsync(
            request.UserName,
            request.Email,
            request.Password,
            request.AvatarUrl
            );
    }
}
