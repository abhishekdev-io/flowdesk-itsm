import { z } from "zod";

export interface Workspace {
  id: string;
  name: string;
  description: string;
  createdAt: string;
}

export const createWorkspaceSchema = z.object({
  Name: z
    .string()
    .min(2, "Name must be at least 2 characters")
    .max(100, "Name must be under 100 characters"),
  Description: z.string().max(500, "Description must be under 500 characters"),
});

export type CreateWorkspaceValues = z.infer<typeof createWorkspaceSchema>;

export type WorkspaceUserResponse = {
  userId: string;
  username: string;
  email: string;
  role: string;
  avatarUrl: string;
};

export interface UpdateWorkspaceRequest {
  name: string;
  description: string;
}

export interface AddMemberRequest {
  userId: string;
  role: WorkspaceRole;
}

export interface ChangeRoleRequest {
  role: WorkspaceRole;
}

export const WorkspaceRole = {
  Member: "Member",
  Admin: "Admin",
  Agent: "Agent",
} as const;

export type WorkspaceRole = (typeof WorkspaceRole)[keyof typeof WorkspaceRole];

export function parseWorkspaceRole(value: string): WorkspaceRole | null {
  return (Object.values(WorkspaceRole) as string[]).includes(value)
    ? (value as WorkspaceRole)
    : null;
}
