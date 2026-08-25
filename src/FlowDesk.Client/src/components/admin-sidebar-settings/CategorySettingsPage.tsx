import AssetCategoryPage from "@/features/asset-category/AssetCategoryPage";

const CategorySettingsPage = () => (
  <div className="mx-auto max-w-4xl p-6">
    <h1 className="mb-1 text-lg font-semibold">Asset categories</h1>
    <p className="mb-6 text-sm text-muted-foreground">
      Manage the categories used to classify assets in this workspace.
    </p>
    <AssetCategoryPage />
  </div>
);

export default CategorySettingsPage;
