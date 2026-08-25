export interface AssetCategoryResponse {
  id: number;
  name: string;
  prefix: string;
  workspaceId: string;
}

export interface DeactivatedAssetCategoryResponse {
  id: number;
  name: string;
  prefix: string;
  deactivatedAt: string;
}

export interface AssetCategoryCreateRequest {
  name: string;
  prefix: string;
}
export interface AssetCategoryUpdateRequest {
  name: string;
  prefix: string;
}
