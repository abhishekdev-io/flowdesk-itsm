import { create } from "zustand";
import {
  parseWorkspaceRole,
  type WorkspaceRole ,
} from "@/types/workspace.types";
import { useAuth } from "./useAuth";
import { workspaceServices } from "@/services/workspaceServices";
import { useShallow } from "zustand/react/shallow";

type WorkspaceContextData =
  | {
      status: "idle";
      activeWorkspaceId: null;
      myRole: null;
      error: null;
    }
  | {
      status: "loading";
      activeWorkspaceId: string;
      myRole: null;
      error: null;
    }
  | {
      status: "ready";
      activeWorkspaceId: string;
      myRole: WorkspaceRole;
      error: null;
    }
  | {
      status: "error";
      activeWorkspaceId: string;
      myRole: null;
      error: string;
    };

type WorkspaceContextState = {
  data: WorkspaceContextData;
  enterWorkspace: (workspaceId: string) => Promise<void>;
  leaveWorkspace: () => void;
};

let activeController: AbortController | null = null;

export const useWorkspaceContext = create<WorkspaceContextState>(
  (set, get) => ({
    data: {
      status: "idle",
      activeWorkspaceId: null,
      myRole: null,
      error: null,
    },

    enterWorkspace: async (workspaceId: string) => {
      const current = get().data;

      if (
        current.activeWorkspaceId === workspaceId &&
        current.status === "ready"
      ) {
        return;
      }

      const auth = useAuth.getState().auth;
      const userId = auth.status === "authenticated" ? auth.user.userId : null;

      if (!userId) {
        set({
          data: {
            status: "error",
            activeWorkspaceId: workspaceId,
            myRole: null,
            error: "Not authenticated",
          },
        });
        return;
      }

      activeController?.abort();
      const controller = new AbortController();
      activeController = controller;

      set({
        data: {
          status: "loading",
          activeWorkspaceId: workspaceId,
          myRole: null,
          error: null,
        },
      });

      try {
        const membership = await workspaceServices.getMyMembership(
          workspaceId,
          userId,
          controller.signal,
        );

        if (controller.signal.aborted) return;

        const role = parseWorkspaceRole(membership.role);
        if (!role) throw new Error(`Unknown role: ${membership.role}`);

        set({
          data: {
            status: "ready",
            activeWorkspaceId: workspaceId,
            myRole: role,
            error: null,
          },
        });
      } catch (err) {
        if (controller.signal.aborted) return;

        const message =
          err instanceof Error
            ? err.message
            : "Failed to load workspace membership";
        console.error("Failed to enter workspace", workspaceId, err);

        set({
          data: {
            status: "error",
            activeWorkspaceId: workspaceId,            
            myRole: null,
            error: message,
          },
        });
      }
    },
    leaveWorkspace: () => {
      activeController?.abort();
      activeController = null;
      set({
        data: {
          status: "idle",
          activeWorkspaceId: null,
          myRole: null,
          error: null,
        },
      });
    },
  }),
);

export const useWorkspaceStatus = () =>
  useWorkspaceContext((s) => s.data.status);

export const useActiveWorkspaceId = () =>
  useWorkspaceContext((s) => s.data.activeWorkspaceId);

export const useMyWorkspaceRole = () =>
  useWorkspaceContext((s) => s.data.myRole);

export const useWorkspaceError = () => useWorkspaceContext((s) => s.data.error);

export const useWorkspaceReady = () =>
  useWorkspaceContext(
    useShallow((s) =>
      s.data.status === "ready"
        ? { ready: true as const, role: s.data.myRole }
        : { ready: false as const, role: null },
    ),
  );
