import { globalUserServices } from "@/services/globalUserServices";
import { useQuery } from "@tanstack/react-query";

export const useGlobalUsers = (workspaceId: string) =>
  useQuery({
    queryKey: ["global-users", workspaceId],
    queryFn: () => globalUserServices.list(workspaceId),
    enabled: !!workspaceId,
  });