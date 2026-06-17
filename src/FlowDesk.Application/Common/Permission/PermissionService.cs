using FlowDesk.Application.Interface;

namespace FlowDesk.Application.Common.Permission;

public class PermissionService(ICurrentWorkspaceContext workspaceContext)
{
    private readonly ICurrentWorkspaceContext _workspaceContext = workspaceContext;

//Asset Permissions
    public bool CanManageAssets()
    => _workspaceContext.IsAdmin;

    public bool CanViewAssets()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

//Category Permissions    
    public bool CanManageCategories()
    => _workspaceContext.IsAdmin;

    public bool CanViewCategories()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

//Ticket Permissions    
    
    public bool CanCreateTickets()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;
    
    public bool CanAssignAndUnAssignTickets()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent;

    public bool CanChangeTicketStatus()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

    public bool CanChangeTicketPriority()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

    public bool CanChangeTicketType()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

    public bool CanArchiveAndUnArchiveTickets()
    => _workspaceContext.IsAdmin;

//TicketComment
    public bool CanViewActiveComments()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

    public bool CanViewDeletedComments()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent;

    public bool CanRestoreComments()
    => _workspaceContext.IsAdmin;

//Workspace Permissions
    public bool CanViewWorkspace()
    => _workspaceContext.IsAdmin || _workspaceContext.IsAgent || _workspaceContext.IsMember;

    public bool CanManageWorkspace()
    => _workspaceContext.IsAdmin;
}


 