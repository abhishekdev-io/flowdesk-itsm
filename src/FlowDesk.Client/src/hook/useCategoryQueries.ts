import { assetCategoryServices } from "@/services/assetCategoryServices";
import type {
  AssetCategoryCreateRequest,
  AssetCategoryUpdateRequest,
} from "@/types/assetCategory.types";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

export const useAssetCategories = (workspaceId: string) =>
  useQuery({
    queryKey: ["categories", workspaceId],
    queryFn: () => assetCategoryServices.list(workspaceId),
    enabled: !!workspaceId,
  });

export const useDeactivatedAssetCategories = (workspaceId: string) =>
  useQuery({
    queryKey: ["decommissioned-categories", workspaceId],
    queryFn: () => assetCategoryServices.listDeactivated(workspaceId),
    enabled: !!workspaceId,
  });

export const useAssetCategory = (workspaceId: string, categoryId: number) =>
  useQuery({
    queryKey: ["category", workspaceId, categoryId],
    queryFn: () => assetCategoryServices.getById(workspaceId, categoryId),
    enabled: !!workspaceId && categoryId !== undefined && categoryId !== null,
  });

export const useCreateAssetCategory = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: AssetCategoryCreateRequest) =>
      assetCategoryServices.create(workspaceId, payload),
    onSuccess: () =>
      qc.invalidateQueries({ queryKey: ["categories", workspaceId] }),
  });
};

export const useUpdateAssetCategory = (
  workspaceId: string,
  categoryId: number,
) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: AssetCategoryUpdateRequest) =>
      assetCategoryServices.update(workspaceId, categoryId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["categories", workspaceId] });
      qc.invalidateQueries({ queryKey: ["category", workspaceId, categoryId] });
    },
  });
};

export const useDeactivateAssetCategory = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (categoryId: number) =>
      assetCategoryServices.deactivate(workspaceId, categoryId),
    onSuccess: (_, categoryId) => {
      qc.invalidateQueries({ queryKey: ["categories", workspaceId] });
      qc.invalidateQueries({
        queryKey: ["decommissioned-categories", workspaceId],
      });
      qc.invalidateQueries({ queryKey: ["category", workspaceId, categoryId] });
    },
  });
};

export const useRestoreAssetCategory = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (categoryId: number) =>
      assetCategoryServices.restore(workspaceId, categoryId),
    onSuccess: (_, categoryId) => {
      qc.invalidateQueries({ queryKey: ["categories", workspaceId] });
      qc.invalidateQueries({
        queryKey: ["decommissioned-categories", workspaceId],
      });
      qc.invalidateQueries({ queryKey: ["category", workspaceId, categoryId] });
    },
  });
};
