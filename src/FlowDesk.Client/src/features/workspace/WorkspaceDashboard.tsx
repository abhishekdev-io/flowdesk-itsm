import { Outlet, useParams } from "react-router";
import { useEffect } from "react";
import {
  useWorkspaceContext,
  useWorkspaceError,
  useWorkspaceStatus,
  useMyWorkspaceRole,
} from "@/stores/useWorkspaceContext";
import Sidebar from "@/components/Sidebar";
import RoleBadge from "@/features/workspace/components/RoleBadge";

const WorkspaceDashboard = () => {
  const { workspaceId } = useParams();
  const { enterWorkspace, leaveWorkspace } = useWorkspaceContext();
  const status = useWorkspaceStatus();
  const error = useWorkspaceError();
  const role = useMyWorkspaceRole();

  useEffect(() => {
    if (workspaceId) enterWorkspace(workspaceId);
    return () => leaveWorkspace();
  }, [enterWorkspace, leaveWorkspace, workspaceId]);

  return (
    <div className="flex h-full bg-background text-foreground">
      <Sidebar />

      <div className="flex flex-1 flex-col overflow-hidden">
        <div className="flex h-11 shrink-0 items-center justify-between border-b border-border/60 bg-background/80 px-5 backdrop-blur supports-backdrop-filter:bg-background/60">
          <div className="flex items-center gap-2 font-mono text-[10px] uppercase tracking-widest text-muted-foreground">
            <span
              className={`h-1.5 w-1.5 rounded-full ${
                status === "error"
                  ? "bg-destructive"
                  : status === "ready"
                  ? "bg-emerald-500"
                  : "animate-pulse bg-amber-500"
              }`}
            />
            {status === "error"
              ? "Connection lost"
              : status === "ready"
              ? "Workspace synced"
              : "Syncing workspace…"}
          </div>

          {role && <RoleBadge role={role} />}
        </div>

        <main className="flex-1 overflow-y-auto">
          {status === "error" ? (
            <div className="flex h-full flex-col items-center justify-center gap-2 px-4 text-center">
              <p className="text-sm font-medium">Couldn't load this workspace</p>
              <p className="text-xs text-muted-foreground">{error}</p>
              <button
                onClick={() => workspaceId && enterWorkspace(workspaceId)}
                className="mt-2 rounded-md border border-border px-3 py-1.5 text-xs transition-colors hover:bg-accent/50"
              >
                Retry
              </button>
            </div>
          ) : (
            <div key={status} className="animate-in fade-in duration-200">
              <Outlet />
            </div>
          )}
        </main>
      </div>
    </div>
  );
};

export default WorkspaceDashboard;
 