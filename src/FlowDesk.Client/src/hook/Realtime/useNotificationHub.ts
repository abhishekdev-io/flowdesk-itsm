import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { createNotificationConnection } from "@/Realtime/notificationHub";

export const useNotificationHub = (workspaceId: string) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!workspaceId) return;

    const connection = createNotificationConnection(workspaceId);

    // Listen for notifications from the backend
    const handleNotification = () => {
      void queryClient.invalidateQueries({
        queryKey: ["notifications", workspaceId],
      });
    };

    connection.on("NotificationReceived", handleNotification);

    // Establish the SignalR connection
    connection
      .start()
      //.then(() => console.log("SignalR connected"))
      .catch((err) => console.error("SignalR failed:", err));

    // Cleanup when the component unmounts or dependencies change
    return () => {
      connection.off("NotificationReceived", handleNotification);
      void connection.stop();
    };
  }, [workspaceId, queryClient]);
};
