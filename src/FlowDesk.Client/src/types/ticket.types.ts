import type { WorkspaceRole, WorkspaceUserResponse } from "./workspace.types";

export interface TicketSummaryResponse {
  id: string;
  title: string;
  ticketNumber: string;
  assetName?: string;
  createdByUserName: string;
  assignedToUserName?: string;
  type: "Incident" | "ServiceRequest" | "Problem" | "ChangeRequest";
  status: "Open" | "InProgress" | "Resolved" | "Closed";
  priority: "Low" | "Medium" | "High" | "Critical";
  riskLevel: "NoSla" | "OnTrack" | "AtRisk" | "Critical" | "Breached";
  archivedAt: string;
  createdAt: string;
}

export interface TicketDetailResponse {
  id: string;
  title: string;
  ticketNumber: string;
  description: string;
  assetName?: string;
  type: "Incident" | "ServiceRequest" | "Problem" | "ChangeRequest";
  status: "Open" | "InProgress" | "Resolved" | "Closed";
  priority: "Low" | "Medium" | "High" | "Critical";
  riskLevel: "NoSla" | "OnTrack" | "AtRisk" | "Critical" | "Breached";
  timeRemaining: string;
  createdByUser: WorkspaceUserResponse;
  assignedToUser?: WorkspaceUserResponse;
  createdAt: string;
  archivedAt: string;
  updatedAt: string;
  slaDeadlineAt: string;
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  assetId?: string;
  type: TicketDetailResponse["type"];
  priority: TicketDetailResponse["priority"];
}

export interface UpdateTicketRequest {
  title: string;
  description: string;
}

export interface TicketFilterParams {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
  priority?: TicketSummaryResponse["priority"];
  status?: TicketSummaryResponse["status"];
  type?: TicketSummaryResponse["type"];
  search?: string;
  isArchived?: boolean;
  assignedToUserId?: string;
  isUnassigned?: boolean;
}

export interface SlaDashboardResponse {
  total: number;
  breached: number;
  critical: number;
  atRisk: number;
  onTrack: number;
  noSla: number;
  compliancePercent: number;
}

export interface TicketActivityResponse {
  id: string;
  performedBy: {
    username: string;
    role: WorkspaceRole;
    avatarUrl: string;
  };
  occurredAt: string;
  action: string;
  description: string;
}

export const TicketActivityType = {
  Created: "Created",
  StatusChanged: "StatusChanged",
  Archived: "Archived",
  UnArchived: "UnArchived",
  Assigned: "Assigned",
  Unassigned: "Unassigned",
  Updated: "Updated",
  PriorityChanged: "PriorityChanged",
  TypeChanged: "TypeChanged",
  SlaBreached: "SlaBreached",
  CommentAdded: "CommentAdded",
} as const;

export type TicketActivityType =
  (typeof TicketActivityType)[keyof typeof TicketActivityType];

export interface AssignTicketRequest {
  assigneeId: string;
}

export interface PriorityRequest {
  priority: TicketSummaryResponse["priority"];
}

export interface TypeRequest {
  type: TicketSummaryResponse["type"];
}

export interface TicketCommentResponse {
  id: string;
  description: string;
  author: {
    userId: string;
    username: string;
    role: WorkspaceRole;
    avatarUrl: string;
  };
  createdAt: string;
  editedAt?: string;
  isEdited?: boolean;
}

export interface DeletedticketCommentResponse {
  id: string;
  description: string;
  author: {
    userId: string;
    username: string;
    role: WorkspaceRole;
    avatarUrl: string;
  };
  createdAt: string;
  editedAt?: string;
  isEdited?: boolean;
  deletedAt: string;
}

export interface CreateTicketCommentRequest {
  description: string;
}

export interface UpdateTicketCommentRequest {
  description: string;
}
