import { useMyWorkspaceRole } from "@/stores/useWorkspaceContext";
import { canAccess, type EntityKey } from "@/config/entityAccess";

export const useEntityAccess = (entity: EntityKey) => {
  const myRole = useMyWorkspaceRole();
  return {
    canView: canAccess(entity, "nav", myRole),
    canCreate: canAccess(entity, "canCreate", myRole),
    canUpdate: canAccess(entity, "canUpdate", myRole),
    myRole,
  };
}; 