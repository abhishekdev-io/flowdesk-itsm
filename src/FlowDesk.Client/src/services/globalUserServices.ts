import apiClient from "@/lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";
import type { User } from "@/types/user";

export const globalUserServices = {
  list: async (workspaceId: string): Promise<User[]> => {
    try {
      const { data } = await apiClient.get<User[]>("/users", {
        headers: {
          "X-Workspace-Id": workspaceId,
        },
      });
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
