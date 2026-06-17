namespace FlowDesk.Application.Common.Permission;

public enum Permissions
{
    CanManageCategories,
    CanViewCategories,

    CanManageAssets,
    CanViewAssets,

    CanCreateTickets,
    CanChangeTicketStatus,
    CanChangeTicketPriority,
    CanChangeTicketType,
    CanAssignAndUnAssignTickets,
    CanArchiveAndUnArchiveTickets,
    CanViewActiveComments,
    CanViewDeletedComments,
    CanRestoreComments,

    CanViewWorkspace,
    CanManageWorkspace,
}
