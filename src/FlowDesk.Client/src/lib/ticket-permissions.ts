import type { TicketDetailResponse } from "@/types/ticket.types";
import type { WorkspaceRole } from "@/types/workspace.types";

export interface CurrentUser {
  id: string;
  role: WorkspaceRole;
}

type PermissionTicket = Pick<
  TicketDetailResponse,
  "status" | "archivedAt" | "createdByUser" | "assignedToUser"
>;

const isStaff = (user: CurrentUser) =>
  user.role === "Admin" || user.role === "Agent";

const isCreator = (user: CurrentUser, ticket: PermissionTicket) =>
  ticket.createdByUser?.userId === user.id;

export const ticketPermissions = (
  user: CurrentUser,
  ticket: PermissionTicket,
) => {
  const staff = isStaff(user);
  const creator = isCreator(user, ticket);
  const isArchived = !!ticket.archivedAt;

  return {
    //Update,type,priority of ticket
    canEdit: !isArchived && (staff || creator),
    canChangePriority: !isArchived && (staff || creator),
    canChangeType: !isArchived && (staff || creator),

    //Ticket status management
    canResolve: !isArchived && staff && ticket.status === "InProgress",
    canClose:
      !isArchived &&
      (user.role === "Admin" || creator) &&
      ticket.status === "Resolved",
    canArchive:
      !isArchived &&
      (user.role === "Admin" || creator) &&
      ticket.status === "Closed",
    canUnarchive: isArchived && (user.role === "Admin" || creator),

    canReopen:
      !isArchived &&
      (staff || creator) &&
      (ticket.status === "Resolved" || ticket.status === "Closed"),

    // Assignment permissions
    canAssign: !isArchived && staff && ticket.status === "Open",
    canUnassign: !isArchived && staff && !!ticket.assignedToUser,

    // Comments
    canComment: !isArchived,
  };
};
