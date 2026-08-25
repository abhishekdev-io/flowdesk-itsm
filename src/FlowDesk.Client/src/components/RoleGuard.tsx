import type { EntityKey } from "@/config/entityAccess";
import { useEntityAccess } from "@/hook/useEntityAccess";
import { Navigate } from "react-router";

export function RoleGuard({
  entity,
  children,
}: {
  entity: EntityKey;
  children: React.ReactNode;
}) {
  const { canView } = useEntityAccess(entity);
  if (!canView) return <Navigate to="/forbidden" replace />;
  return <>{children}</>;
}
 