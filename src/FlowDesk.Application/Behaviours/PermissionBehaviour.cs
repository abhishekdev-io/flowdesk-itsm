using System.Reflection;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Behaviours;

internal sealed class PermissionBehaviour<TRequest, TResponse>(
    PermissionService permissionService
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IWorkspaceRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
    {
        var attribute = typeof(TRequest)
            .GetCustomAttribute<RequirePermissionAttribute>();

        if(attribute is null) 
           return await next(cancellationToken);

        var allowed = attribute.Permission switch
        {
            Permissions.CanManageAssets                 => permissionService.CanManageAssets(),
            Permissions.CanViewAssets                   => permissionService.CanViewAssets(),

            Permissions.CanManageCategories             => permissionService.CanManageCategories(),
            Permissions.CanViewCategories               => permissionService.CanViewCategories(),

            Permissions.CanCreateTickets                => permissionService.CanCreateTickets(),
            Permissions.CanChangeTicketStatus           => permissionService.CanChangeTicketStatus(),
            Permissions.CanChangeTicketPriority         => permissionService.CanChangeTicketPriority(),
            Permissions.CanChangeTicketType             => permissionService.CanChangeTicketType(),
            Permissions.CanAssignAndUnAssignTickets     => permissionService.CanAssignAndUnAssignTickets(),
            Permissions.CanArchiveAndUnArchiveTickets   => permissionService.CanArchiveAndUnArchiveTickets(),

            Permissions.CanViewActiveComments           => permissionService.CanViewActiveComments(),
            Permissions.CanViewDeletedComments          => permissionService.CanViewDeletedComments(),
            Permissions.CanRestoreComments              => permissionService.CanRestoreComments(),

            Permissions.CanViewWorkspace                => permissionService.CanViewWorkspace(),
            Permissions.CanManageWorkspace              => permissionService.CanManageWorkspace(),
            
            _ => throw new PermissionMethodNotFoundException($"Unhandled permission: {attribute.Permission}")
        }; 

        if (!allowed)
            throw new ForbiddenException("You do not have permission to perform this action.");

         return await next(cancellationToken);        
    }
}
