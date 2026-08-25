import { useState } from "react";
import { Plus } from "lucide-react";
import { useParams } from "react-router";
import { useAssetCategories, useDeactivatedAssetCategories } from "@/hook/useCategoryQueries";
import CategoryTable from "./components/CategoryTable";
import CategoryFormDialog from "./components/CategoryFormDialog";
import { useEntityAccess } from "@/hook/useEntityAccess";

type Tab = "active" | "deactivated";

const AssetCategoryPage = () => {
  const { workspaceId } = useParams();
  const [tab, setTab] = useState<Tab>("active");
  const [createOpen, setCreateOpen] = useState(false);

  const { canCreate, canUpdate } = useEntityAccess("assetcategories");

  const activeQuery = useAssetCategories(workspaceId!);
  const deactivatedQuery = useDeactivatedAssetCategories(workspaceId!);

  return (
    <div className="p-6">
      <div className="mb-5 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">Asset categories</h1>
          <p className="text-sm text-muted-foreground">
            Manage the category taxonomy used to tag assets.
          </p>
        </div>

        {canCreate && (
          <button
            onClick={() => setCreateOpen(true)}
            className="flex items-center gap-1.5 rounded-md bg-primary px-3 py-1.5 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
          >
            <Plus className="h-4 w-4" />
            New category
          </button>
        )}
      </div>

      {/* Tabs */}
      <div className="mb-4 flex items-center gap-1 border-b border-border">
        {(["active", "deactivated"] as Tab[]).map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`relative px-3 py-2 text-sm font-medium capitalize transition-colors ${
              tab === t
                ? "text-foreground"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            {t}
            {tab === t && (
              <span className="absolute inset-x-0 -bottom-px h-0.5 rounded-full bg-primary" />
            )}
          </button>
        ))}
      </div>

      {tab === "active" ? (
        <CategoryTable
          variant="active"
          workspaceId={workspaceId!}
          data={activeQuery.data}
          isLoading={activeQuery.isLoading}
          isError={activeQuery.isError}
          canMutate={canUpdate}
        />
      ) : (
        <CategoryTable
          variant="deactivated"
          workspaceId={workspaceId!}
          data={deactivatedQuery.data}
          isLoading={deactivatedQuery.isLoading}
          isError={deactivatedQuery.isError}
          canMutate={canUpdate}
        />
      )}

      {canCreate && (
        <CategoryFormDialog
          mode="create"
          workspaceId={workspaceId!}
          open={createOpen}
          onOpenChange={setCreateOpen}
        />
      )}
    </div>
  )
}

export default AssetCategoryPage;