export interface NotificationResponse {
  id: string;
  title: string;
  body: string;
  eventType:
    | "TicketCreated"
    | "TicketAssigned"
    | "TicketUnassigned"
    | "TicketStatusChanged"
    | "TicketCommentAdded"
    | "TicketPriorityChanged"
    | "AddedToWorkspace"
    | "RemovedFromWorkspace"
    | "WorkspaceRoleChanged";
  relatedTicketId: string;
  isRead: boolean;
  createdAt: string;
}
