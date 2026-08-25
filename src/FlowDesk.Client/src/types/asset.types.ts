export interface AssetSummaryResponse {
  id: string;
  name: string;
  assetTag: string;
  status: "Active" | "UnderMaintenance" | "Decommissioned";
  categoryName: string;
  manufacturer: string;
  createdAt: string;
  decommissionedAt?: string;
}

export interface AssetDetailResponse {
  id: string;
  name: string;
  assetTag: string;
  status: "Active" | "UnderMaintenance" | "Decommissioned";
  category: string;
  manufacturer: string;
  modelnumber: string;
  serialnumber: string;
  createdAt: string;
  updatedAt?: string;
  decommissionedAt?: string;
  workspaceId: string;
}

export interface CreateAssetRequest {
  name: string;
  categoryId: number;
  modelnumber: string;
  manufacturer: string;
  serialnumber: string;
}

export interface UpdateAssetRequest {
  name: string;
  categoryId: number;
  modelnumber: string;
  manufacturer: string;
  serialnumber: string;
}


export interface AssetFilterParams {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  descending?: boolean;
  status?: AssetSummaryResponse["status"];
  search?: string;
  categoryId?: number;
}

export interface AssetStatsResponse {
  total: number;
  active: number;
  maintenance: number;
  decommissioned: number;
}