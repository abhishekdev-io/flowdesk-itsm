import apiClient from "@/lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";
import type { PagedResult } from "@/types/pagedResults";
import type {
  AssignTicketRequest,
  CreateTicketCommentRequest,
  CreateTicketRequest,
  DeletedticketCommentResponse,
  PriorityRequest,
  SlaDashboardResponse,
  TicketActivityResponse,
  TicketCommentResponse,
  TicketDetailResponse,
  TicketFilterParams,
  TicketSummaryResponse,
  TypeRequest,
  UpdateTicketCommentRequest,
  UpdateTicketRequest,
} from "@/types/ticket.types";

export const ticketServices = {
  list: async (
    workspaceId: string,
    filters: TicketFilterParams = {},
  ): Promise<PagedResult<TicketSummaryResponse>> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets`,
        { params: filters },
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  getById: async (
    workspaceId: string,
    ticketId: string,
  ): Promise<TicketDetailResponse> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets/${ticketId}`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  getSlaDashboard: async (
    workspaceId: string,
  ): Promise<SlaDashboardResponse> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets/sla-dashboard`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  create: async (
    workspaceId: string,
    payload: CreateTicketRequest,
  ): Promise<string> => {
    try {
      const { data } = await apiClient.post(
        `/workspaces/${workspaceId}/tickets`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  update: async (
    workspaceId: string,
    ticketId: string,
    payload: UpdateTicketRequest,
  ): Promise<string> => {
    try {
      const { data } = await apiClient.put(
        `/workspaces/${workspaceId}/tickets/${ticketId}`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  getActivityLog: async (
    workspaceId: string,
    ticketId: string,
  ): Promise<TicketActivityResponse[]> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets/${ticketId}/activities`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  archive: async (workspaceId: string, ticketId: string): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/archive`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  unArchive: async (workspaceId: string, ticketId: string): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/unarchive`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  assignUser: async (
    workspaceId: string,
    ticketId: string,
    payload: AssignTicketRequest,
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/assign`,
        payload,
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  unAssignUser: async (
    workspaceId: string,
    ticketId: string,
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/unassign`,
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  priority: async (
    workspaceId: string,
    ticketId: string,
    payload: PriorityRequest,
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/priority`,
        payload,
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  type: async (
    workspaceId: string,
    ticketId: string,
    payload: TypeRequest,
  ): Promise<void> => {
    try {
      await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/type`,
        payload,
      );
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  close: async (workspaceId: string, ticketId: string): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/close`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  reopen: async (workspaceId: string, ticketId: string): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/reopen`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  resolve: async (workspaceId: string, ticketId: string): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/resolve`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  listComments: async (
    workspaceId: string,
    ticketId: string,
  ): Promise<PagedResult<TicketCommentResponse>> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets/${ticketId}/comments`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  listDeletedComments: async (
    workspaceId: string,
    ticketId: string,
  ): Promise<PagedResult<DeletedticketCommentResponse>> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/tickets/${ticketId}/deleted-comments`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  createComment: async (
    workspaceId: string,
    ticketId: string,
    payload: CreateTicketCommentRequest,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.post(
        `/workspaces/${workspaceId}/tickets/${ticketId}/comments`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  updateComment: async (
    workspaceId: string,
    ticketId: string,
    commentId: string,
    payload: UpdateTicketCommentRequest,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.put(
        `/workspaces/${workspaceId}/tickets/${ticketId}/comments/${commentId}`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  deleteComment: async (
    workspaceId: string,
    ticketId: string,
    commentId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.delete(
        `/workspaces/${workspaceId}/tickets/${ticketId}/comments/${commentId}`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },

  restoreComment: async (
    workspaceId: string,
    ticketId: string,
    commentId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/tickets/${ticketId}/comments/${commentId}/restore`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
