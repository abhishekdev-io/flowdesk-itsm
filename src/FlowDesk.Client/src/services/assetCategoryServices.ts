import apiClient from "@/lib/apiClient";
import extractErrorMessage from "@/lib/extractErrorMessage";
import type {
  AssetCategoryCreateRequest,
  AssetCategoryResponse,
  AssetCategoryUpdateRequest,
  DeactivatedAssetCategoryResponse,
} from "@/types/assetCategory.types";

export const assetCategoryServices = {
  list: async (workspaceId: string): Promise<AssetCategoryResponse[]> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/categories`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  listDeactivated: async (
    workspaceId: string,
  ): Promise<DeactivatedAssetCategoryResponse[]> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/categories/deactivated`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  getById: async (
    workspaceId: string,
    categoryId: number,
  ): Promise<AssetCategoryResponse> => {
    try {
      const { data } = await apiClient.get(
        `/workspaces/${workspaceId}/categories/${categoryId}`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  create: async (
    workspaceId: string,
    payload: AssetCategoryCreateRequest,
  ): Promise<number> => {
    try {
      const { data } = await apiClient.post(
        `/workspaces/${workspaceId}/categories`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  update: async (
    workspaceId: string,
    categoryId: number,
    payload: AssetCategoryUpdateRequest,
  ): Promise<number> => {
    try {
      const { data } = await apiClient.put(
        `/workspaces/${workspaceId}/categories/${categoryId}`,
        payload,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  deactivate: async (
    workspaceId: string,
    categoryId: number,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/categories/${categoryId}/deactivate`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
  restore: async (
    workspaceId: string,
    categoryId: number,
  ): Promise<void> => {
    try {
      const { data } = await apiClient.patch(
        `/workspaces/${workspaceId}/categories/${categoryId}/restore`,
      );
      return data;
    } catch (err) {
      throw new Error(extractErrorMessage(err), { cause: err });
    }
  },
};
