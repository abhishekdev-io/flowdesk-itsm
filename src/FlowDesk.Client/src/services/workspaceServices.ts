import type {
  AddMemberRequest,
  ChangeRoleRequest,
  CreateWorkspaceValues,
  UpdateWorkspaceRequest,
  Workspace,
  WorkspaceUserResponse,
} from "@/types/workspace.types";
import apiClient from "../lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";

export const workspaceServices = {
  list: async (): Promise<Workspace[]> => {
    try {
      const response = await apiClient.get<Workspace[]>("/workspaces");
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  getById: async (workspaceId: string): Promise<Workspace> => {
    try {
      const response = await apiClient.get<Workspace>(
        `/workspaces/${workspaceId}`,
      );
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  create: async (payload: CreateWorkspaceValues): Promise<string> => {
    try {
      const response = await apiClient.post("/workspaces", payload);
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  update: async (
    workspaceId: string,
    payload: UpdateWorkspaceRequest,
  ): Promise<string> => {
    try {
      const response = await apiClient.put(
        `/workspaces/${workspaceId}`,
        payload,
      );
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  delete: async (
    workspaceId: string,
  ): Promise<void> => {
    try {
      const response = await apiClient.delete(
        `/workspaces/${workspaceId}`
      );
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  getMyMembership: async (
    workspaceId: string,
    userId: string,
    signal?: AbortSignal,
  ): Promise<WorkspaceUserResponse> => {
    try {
      const response = await apiClient.get<WorkspaceUserResponse>(
        `/workspaces/${workspaceId}/members/${userId}`,
        { signal },
      );
      return response.data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  listMembers: async (
    workspaceId: string,
  ): Promise<WorkspaceUserResponse[]> => {
    try {
      const { data } = await apiClient.get<WorkspaceUserResponse[]>(
        `/workspaces/${workspaceId}/members`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  removeMember: async (workspaceId: string, userId: string): Promise<void> => {
    try {
      await apiClient.delete(`/workspaces/${workspaceId}/members/${userId}`);
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  addMember: async (
    workspaceId: string,
    payload: AddMemberRequest,
  ): Promise<void> => {
    try {
      await apiClient.post(`/workspaces/${workspaceId}/members`, payload);
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  changeMemberRole: async (
    workspaceId: string,
    userId: string,
    payload: ChangeRoleRequest,
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/members/${userId}/change-role`,
        payload,
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
