import { notificationServices } from "@/services/notificationServices";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useNotifications = (workspaceId: string) =>
  useQuery({
    queryKey: ["notifications", workspaceId],
    queryFn: () => notificationServices.list(workspaceId),
    enabled: !!workspaceId,
  });

export const useMarkNotificationRead = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (notificationIds: string[]) =>
      notificationServices.markAsRead(workspaceId, notificationIds),
    onSuccess: () =>
      qc.invalidateQueries({ queryKey: ["notifications", workspaceId] }),
  });
};

export const useDeleteNotifications = (workspaceId: string) => {
  const qc = useQueryClient();

  return useMutation({
    mutationFn: (notificationIds: string[]) =>
      notificationServices.delete(workspaceId, notificationIds),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["notifications", workspaceId] });
    },
  });
};
