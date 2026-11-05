import { useAuth } from "@/stores/useAuth";
import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
} from "@microsoft/signalr";

export const createNotificationConnection = (
  workspaceId: string,
): HubConnection => {
  return new HubConnectionBuilder()
    .withUrl(
      `${import.meta.env.VITE_API_BASE_URL}/hubs/notifications?workspaceId=${workspaceId}`,
      {
        accessTokenFactory: () => useAuth.getState().accessToken ?? "",
      },
    )
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
};
