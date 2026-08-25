import { assetServices } from "@/services/assetServices";
import type {
  AssetFilterParams,
  CreateAssetRequest,
  UpdateAssetRequest,
} from "@/types/asset.types";
import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { toast } from "sonner";

export const useAssets = (workspaceId: string, filters: AssetFilterParams) =>
  useQuery({
    queryKey: ["assets", workspaceId, filters],
    queryFn: () => assetServices.list(workspaceId, filters),
    enabled: !!workspaceId,
    placeholderData: keepPreviousData,
  });

export const useAsset = (workspaceId: string, assetId: string) =>
  useQuery({
    queryKey: ["asset", workspaceId, assetId],
    queryFn: () => assetServices.getById(workspaceId, assetId),
    enabled: !!workspaceId && !!assetId,
  });

export const useAssetStats = (workspaceId: string) =>
  useQuery({
    queryKey: ["asset", "stats", workspaceId],
    queryFn: () => assetServices.getAssetStats(workspaceId),
    enabled: !!workspaceId,
  });

export const useCreateAsset = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateAssetRequest) =>
      assetServices.create(workspaceId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      toast.success("Asset created successfully");
    },
    onError: (err: Error) => {
      toast.error(err.message || "Failed to create asset");
    },
  });
};

export const useUpdateAsset = (workspaceId: string, assetId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: UpdateAssetRequest) =>
      assetServices.update(workspaceId, assetId, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      qc.invalidateQueries({ queryKey: ["asset", workspaceId, assetId] });
      toast.success("Asset updated");
    },
    onError: (err: Error) =>
      toast.error(err.message || "Couldn't update asset"),
  });
};

export const useRestoreAsset = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (assetId: string) =>
      assetServices.restore(workspaceId, assetId),
    onSuccess: (_, assetId) => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      qc.invalidateQueries({ queryKey: ["asset", workspaceId, assetId] });
      qc.invalidateQueries({ queryKey: ["asset", "stats", workspaceId] });
      toast.success("Asset returned to active");
    },
    onError: (err: Error) =>
      toast.error(err.message || "Couldn't update asset status"),
  });
};

export const useDecommissionAsset = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (assetId: string) =>
      assetServices.decommission(workspaceId, assetId),
    onSuccess: (_, assetId) => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      qc.invalidateQueries({ queryKey: ["asset", workspaceId, assetId] });
      qc.invalidateQueries({ queryKey: ["asset", "stats", workspaceId] });
      toast.success("Asset decommissioned");
    },
    onError: (err: Error) =>
      toast.error(err.message || "Couldn't update asset status"),
  });
};

export const useMoveToMaintenance = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (assetId: string) =>
      assetServices.underMaintenance(workspaceId, assetId),
    onSuccess: (_, assetId) => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      qc.invalidateQueries({ queryKey: ["asset", workspaceId, assetId] });
      qc.invalidateQueries({ queryKey: ["asset", "stats", workspaceId] });
      toast.success("Asset moved to maintenance");
    },
    onError: (err: Error) =>
      toast.error(err.message || "Couldn't update asset status"),
  });
};

export const useReturnFromMaintenance = (workspaceId: string) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (assetId: string) =>
      assetServices.returnFromMaintenance(workspaceId, assetId),
    onSuccess: (_, assetId) => {
      qc.invalidateQueries({ queryKey: ["assets", workspaceId] });
      qc.invalidateQueries({ queryKey: ["asset", workspaceId, assetId] });
      qc.invalidateQueries({ queryKey: ["asset", "stats", workspaceId] });
      toast.success("Asset returned to active");
    },
    onError: (err: Error) =>
      toast.error(err.message || "Couldn't update asset status"),
  });
};
