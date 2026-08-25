import { ticketServices } from "@/services/ticketServices";
import type {
  AssignTicketRequest,
  CreateTicketCommentRequest,
  CreateTicketRequest,
  PriorityRequest,
  TicketFilterParams,
  TypeRequest,
  UnassignTicketRequest,
  UpdateTicketCommentRequest,
  UpdateTicketRequest,
} from "@/types/ticket.types";
import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { toast } from "sonner";

export const ticketKeys = {
  list: (workspaceId: string, filters?: TicketFilterParams) =>
    filters ? (["tickets", workspaceId, filters] as const) : (["tickets", workspaceId] as const),
  detail: (workspaceId: string, ticketId: string) =>
    ["ticket", workspaceId, ticketId] as const,
  activity: (workspaceId: string, ticketId: string) =>
    ["activities", workspaceId, ticketId] as const,
  comments: (workspaceId: string, ticketId: string) =>
    ["comments", workspaceId, ticketId] as const,
  deletedComments: (workspaceId: string, ticketId: string) =>
    ["deleted-comments", workspaceId, ticketId] as const,
  slaDashboard: (workspaceId: string) =>
    ["sla-dashboard", workspaceId] as const,
};

// --- Core ticket queries ---

export const useTickets = (workspaceId: string, filters: TicketFilterParams) =>
  useQuery({
    queryKey: ticketKeys.list(workspaceId, filters),
    queryFn: () => ticketServices.list(workspaceId, filters),
    enabled: !!workspaceId,
    placeholderData: keepPreviousData,
  });

export const useTicket = (workspaceId: string, ticketId: string) =>
  useQuery({
    queryKey: ticketKeys.detail(workspaceId, ticketId),
    queryFn: () => ticketServices.getById(workspaceId, ticketId),
    enabled: !!workspaceId && !!ticketId,
  });

export const useSlaDashboard = (workspaceId: string) =>
  useQuery({
    queryKey: ticketKeys.slaDashboard(workspaceId),
    queryFn: () => ticketServices.getSlaDashboard(workspaceId),
    enabled: !!workspaceId,
  });

export const useCreateTicket = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateTicketRequest) =>
      ticketServices.create(workspaceId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      toast.success("Ticket created");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to create ticket",
      );
    },
  });
};

export const useUpdateTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: UpdateTicketRequest) =>
      ticketServices.update(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      toast.success("Ticket updated");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to update ticket",
      );
    },
  });
};

export const useTicketActivity = (workspaceId: string, ticketId: string) =>
  useQuery({
    queryKey: ticketKeys.activity(workspaceId, ticketId),
    queryFn: () => ticketServices.getActivityLog(workspaceId, ticketId),
    enabled: !!workspaceId && !!ticketId,
  });

// --- Status / assignment / classification mutations ---

export const useArchiveTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => ticketServices.archive(workspaceId, ticketId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket archived");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to archive ticket",
      );
    },
  });
};

export const useUnArchiveTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => ticketServices.unArchive(workspaceId, ticketId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket unarchived");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to unarchive ticket",
      );
    },
  });
};

export const useAssignTicketUser = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: AssignTicketRequest) =>
      ticketServices.assignUser(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket assigned");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to assign ticket",
      );
    },
  });
};

export const useUnAssignTicketUser = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: UnassignTicketRequest) =>
      ticketServices.unAssignUser(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket unassigned");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to unassign ticket",
      );
    },
  });
};

export const useUpdateTicketPriority = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: PriorityRequest) =>
      ticketServices.priority(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Priority updated");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to update priority",
      );
    },
  });
};

export const useUpdateTicketType = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: TypeRequest) =>
      ticketServices.type(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Type updated");
    },
    onError: (err: Error) => {
      toast.error(err instanceof Error ? err.message : "Failed to update type");
    },
  });
};

export const useCloseTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => ticketServices.close(workspaceId, ticketId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket closed");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to close ticket",
      );
    },
  });
};

export const useReopenTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => ticketServices.reopen(workspaceId, ticketId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket reopened");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to reopen ticket",
      );
    },
  });
};

export const useResolveTicket = (workspaceId: string, ticketId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => ticketServices.resolve(workspaceId, ticketId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.list(workspaceId) });
      qc.invalidateQueries({ queryKey: ticketKeys.detail(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Ticket resolved");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to resolve ticket",
      );
    },
  });
};

// --- Comments ---

export const useTicketComments = (workspaceId: string, ticketId: string) =>
  useQuery({
    queryKey: ticketKeys.comments(workspaceId, ticketId),
    queryFn: () => ticketServices.listComments(workspaceId, ticketId),
    enabled: !!workspaceId && !!ticketId,
  });

export const useTicketDeletedComments = (
  workspaceId: string,
  ticketId: string,
) =>
  useQuery({
    queryKey: ticketKeys.deletedComments(workspaceId, ticketId),
    queryFn: () => ticketServices.listDeletedComments(workspaceId, ticketId),
    enabled: !!workspaceId && !!ticketId,
  });

export const useCreateTicketComment = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateTicketCommentRequest) =>
      ticketServices.createComment(workspaceId, ticketId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.comments(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.activity(workspaceId, ticketId) });
      toast.success("Comment added");
    },
    onError: (err: Error) => {
      toast.error(err instanceof Error ? err.message : "Failed to add comment");
    },
  });
};

export const useUpdateTicketComment = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({
      commentId,
      payload,
    }: {
      commentId: string;
      payload: UpdateTicketCommentRequest;
    }) =>
      ticketServices.updateComment(workspaceId, ticketId, commentId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.comments(workspaceId, ticketId) });
      toast.success("Comment updated");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to update comment",
      );
    },
  });
};

export const useDeleteTicketComment = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (commentId: string) =>
      ticketServices.deleteComment(workspaceId, ticketId, commentId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.comments(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.deletedComments(workspaceId, ticketId) });
      toast.success("Comment deleted");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to delete comment",
      );
    },
  });
};

export const useRestoreTicketComment = (
  workspaceId: string,
  ticketId: string,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (commentId: string) =>
      ticketServices.restoreComment(workspaceId, ticketId, commentId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ticketKeys.comments(workspaceId, ticketId) });
      qc.invalidateQueries({ queryKey: ticketKeys.deletedComments(workspaceId, ticketId) });
      toast.success("Comment restored");
    },
    onError: (err: Error) => {
      toast.error(
        err instanceof Error ? err.message : "Failed to restore comment",
      );
    },
  });
};
