import { workspaceServices } from "@/services/workspaceServices";
import { useAuth } from "@/stores/useAuth";
import type {
  AddMemberRequest,
  ChangeRoleRequest,
  CreateWorkspaceValues,
  UpdateWorkspaceRequest,
} from "@/types/workspace.types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";

export const workspaceKeys = {
  all: ["workspaces"] as const,
  lists: () => [...workspaceKeys.all, "list"] as const,
  detail: (id: string) => [...workspaceKeys.all, "detail", id] as const,
  members: (workspaceId: string) =>
    [...workspaceKeys.detail(workspaceId), "members"] as const,
  membership: (workspaceId: string, userId: string) =>
    [...workspaceKeys.members(workspaceId), userId] as const,
};

export const useWorkspaces = () =>
  useQuery({
    queryKey: workspaceKeys.lists(),
    queryFn: () => workspaceServices.list(),
  });

export const useWorkspace = (workspaceId: string) =>
  useQuery({
    queryKey: workspaceKeys.detail(workspaceId),
    queryFn: () => workspaceServices.getById(workspaceId),
    enabled: !!workspaceId,
  });

export const useCreateWorkspace = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateWorkspaceValues) =>
      workspaceServices.create(payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: workspaceKeys.lists() });
    },
  });
};

export const useUpdateWorkspace = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: UpdateWorkspaceRequest) =>
      workspaceServices.update(workspaceId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: workspaceKeys.lists() });
      qc.invalidateQueries({ queryKey: workspaceKeys.detail(workspaceId) });
      toast.success("Workspace Updated successfully");
    },
    onError: (err: Error) => {
      toast.error(err.message || "Failed to Update Workspace");
    },
  });
};

export const useGetMyMembership = (workspaceId: string) => {
  const auth = useAuth((state) => state.auth);
  const userId = auth.status === "authenticated" ? auth.user?.userId : null;

  return useQuery({
    queryKey: workspaceKeys.membership(workspaceId, userId ?? ""),
    queryFn: () => workspaceServices.getMyMembership(workspaceId, userId!),
    enabled: !!workspaceId && !!userId,
  });
};

export const useDeleteWorkspace = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => workspaceServices.delete(workspaceId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: workspaceKeys.lists() });
    },
  });
};

export const useWorkspaceMembers = (workspaceId: string) =>
  useQuery({
    queryKey: workspaceKeys.members(workspaceId),
    queryFn: () => workspaceServices.listMembers(workspaceId),
    enabled: !!workspaceId,
  });

export const useAddMember = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: AddMemberRequest) =>
      workspaceServices.addMember(workspaceId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: workspaceKeys.members(workspaceId) });
      toast.success("New member added successfully");
    },
    onError: (err: Error) => {
      toast.error(err.message || "Failed to add member");
    },
  });
};

export const useRemoveMember = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (userId: string) =>
      workspaceServices.removeMember(workspaceId, userId),
    onSuccess: (_, userId) => {
      qc.invalidateQueries({ queryKey: workspaceKeys.members(workspaceId) });
      qc.invalidateQueries({
        queryKey: workspaceKeys.membership(workspaceId, userId),
      });
      toast.success("Member removed successfully");
    },
    onError: (err: Error) => {
      toast.error(err.message || "Failed to remove member");
    },
  });
};

export const useChangeMemberRole = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({
      userId,
      payload,
    }: {
      userId: string;
      payload: ChangeRoleRequest;
    }) => workspaceServices.changeMemberRole(workspaceId, userId, payload),
    onSuccess: (_, { userId }) => {
      qc.invalidateQueries({ queryKey: workspaceKeys.members(workspaceId) });
      qc.invalidateQueries({
        queryKey: workspaceKeys.membership(workspaceId, userId),
      });
    },
  });
};