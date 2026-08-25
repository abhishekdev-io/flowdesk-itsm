import apiClient from "@/lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";
import type { NotificationResponse } from "@/types/notification.types";
import type { PagedResult } from "@/types/pagedResults";

export const notificationServices = {
  list: async (
    workspaceId: string,
  ): Promise<PagedResult<NotificationResponse>> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/notifications`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  markAsRead: async (
    workspaceId: string,
    notificationIds: string[],
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/notifications/mark-read`,
        { notificationIds },
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  delete: async (
    workspaceId: string,
    notificationIds: string[],
  ): Promise<void> => {
    try {
      await apiClient.delete(
        `/workspaces/${workspaceId}/notifications/delete`,
        {
          data: { notificationIds },
        },
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
