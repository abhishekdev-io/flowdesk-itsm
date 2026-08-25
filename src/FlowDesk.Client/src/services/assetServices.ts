import apiClient from "@/lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";
import type {
  AssetDetailResponse,
  AssetFilterParams,
  AssetStatsResponse,
  AssetSummaryResponse,
  CreateAssetRequest,
  UpdateAssetRequest,
} from "@/types/asset.types";
import type { PagedResult } from "@/types/pagedResults";

export const assetServices = {
  list: async (
    workspaceId: string,
    filters: AssetFilterParams = {},
  ): Promise<PagedResult<AssetSummaryResponse>> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/assets`,
        { params: filters },
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  getById: async (
    workspaceId: string,
    assetId: string,
  ): Promise<AssetDetailResponse> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/assets/${assetId}`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  getAssetStats: async (workspaceId: string): Promise<AssetStatsResponse> => {
    try {
      const { data } = await apiClient.get<AssetStatsResponse>(
        `/workspaces/${workspaceId}/assets/stats`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  create: async (
    workspaceId: string,
    payload: CreateAssetRequest,
  ): Promise<string> => {
    try {
      const { data } = await apiClient.post(
        `/workspaces/${workspaceId}/assets`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  update: async (
    workspaceId: string,
    assetId: string,
    payload: UpdateAssetRequest,
  ): Promise<string> => {
    try {
      const { data } = await apiClient.put(
        `/workspaces/${workspaceId}/assets/${assetId}`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  decommission: async (
    workspaceId: string,
    assetId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/assets/${assetId}/decommission`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  restore: async (
    workspaceId: string,
    assetId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/assets/${assetId}/restore`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  underMaintenance: async (
    workspaceId: string,
    assetId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/assets/${assetId}/move-to-maintenance`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  returnFromMaintenance: async (
    workspaceId: string,
    assetId: string,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/assets/${assetId}/return-from-maintenance`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
