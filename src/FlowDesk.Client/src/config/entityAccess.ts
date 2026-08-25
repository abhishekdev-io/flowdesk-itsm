import { WorkspaceRole  } from "@/types/workspace.types";

export const ENTITY_ACCESS = {
  tickets: {
    nav: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canCreate: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canUpdate: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
  },
  notifications: {
    nav: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canCreate: [],
    canUpdate: [],
  },
  archived: {
    nav: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canCreate: [],
    canUpdate: [],
  },
  assets: {
    nav: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canCreate: [WorkspaceRole.Admin],
    canUpdate: [WorkspaceRole.Admin],
  },
  assetcategories: {
    nav: [WorkspaceRole.Admin],
    canCreate: [WorkspaceRole.Admin],
    canUpdate: [WorkspaceRole.Admin],
  },
  members: {
    nav: [WorkspaceRole.Admin, WorkspaceRole.Agent, WorkspaceRole.Member],
    canCreate: [WorkspaceRole.Admin],
    canUpdate: [WorkspaceRole.Admin],
  },
  globalUsers: {
    nav: [WorkspaceRole.Admin], 
    canCreate: [],
    canUpdate: [],
  },
  sladashboard: {
    nav: [WorkspaceRole.Admin],
    canCreate: [],
    canUpdate: [],
  },
  settings: {
    nav: [WorkspaceRole.Admin],
    canCreate: [],
    canUpdate: [],
  },
} as const;

export type EntityKey = keyof typeof ENTITY_ACCESS;

export const canAccess = (
  entity: EntityKey,
  action: "nav" | "canCreate" | "canUpdate",
  role: WorkspaceRole | null,
): boolean => role !== null && (ENTITY_ACCESS[entity][action] as readonly WorkspaceRole[]).includes(role);
