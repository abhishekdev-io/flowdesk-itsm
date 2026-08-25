import { useState } from "react";
import { Pencil, Ban, RotateCcw } from "lucide-react";
import type {
  AssetCategoryResponse,
  DeactivatedAssetCategoryResponse,
} from "@/types/assetCategory.types";
import {
  useDeactivateAssetCategory,
  useRestoreAssetCategory,
} from "@/hook/useCategoryQueries";
import CategoryFormDialog from "./CategoryFormDialog";
import DeactivateCategoryDialog from "./DeactivateCategoryDialog";

interface CategoryTableProps {
  workspaceId: string;
  variant: "active" | "deactivated";
  data?: AssetCategoryResponse[] | DeactivatedAssetCategoryResponse[];
  isLoading: boolean;
  isError: boolean;
  canMutate: boolean;
}

const CategoryTable = ({
  workspaceId,
  variant,
  data,
  isLoading,
  isError,
  canMutate,
}: CategoryTableProps) => {
  const [editingId, setEditingId] = useState<number | null>(null);
  const [deactivatingId, setDeactivatingId] = useState<number | null>(null);

  const deactivateMutation = useDeactivateAssetCategory(workspaceId);
  const restoreMutation = useRestoreAssetCategory(workspaceId);

  if (isLoading) {
    return (
      <div className="rounded-lg border border-border p-8 text-center text-sm text-muted-foreground">
        Loading categories…
      </div>
    );
  }

  if (isError) {
    return (
      <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-8 text-center text-sm text-destructive">
        Couldn't load categories. Try refreshing.
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="rounded-lg border border-dashed border-border p-8 text-center text-sm text-muted-foreground">
        {variant === "active"
          ? "No categories yet."
          : "No deactivated categories."}
      </div>
    );
  }

  const editingCategory =
    variant === "active" && editingId != null
      ? (data as AssetCategoryResponse[]).find((c) => c.id === editingId)
      : undefined;

  return (
    <>
      <div className="overflow-hidden rounded-lg border border-border">
        <table className="w-full text-sm">
          <thead className="bg-muted/40 text-left text-xs uppercase tracking-wide text-muted-foreground">
            <tr>
              <th className="px-4 py-2.5 font-medium">Name</th>
              <th className="px-4 py-2.5 font-medium">Prefix</th>
              {variant === "deactivated" && (
                <th className="px-4 py-2.5 font-medium">Deactivated at</th>
              )}
              {canMutate && <th className="w-24 px-4 py-2.5" />}
            </tr>
          </thead>
          <tbody>
            {data.map((category) => (
              <tr
                key={category.id}
                className="border-t border-border/60 hover:bg-accent/20"
              >
                <td className="px-4 py-2.5 font-medium text-foreground">
                  {category.name}
                </td>
                <td className="px-4 py-2.5 font-mono text-xs text-muted-foreground">
                  {category.prefix}
                </td>
                {variant === "deactivated" && (
                  <td className="px-4 py-2.5 text-muted-foreground">
                    {new Date(
                      (category as DeactivatedAssetCategoryResponse)
                        .deactivatedAt,
                    ).toLocaleDateString()}
                  </td>
                )}
                {canMutate && (
                  <td className="px-4 py-2.5">
                    <div className="flex items-center justify-end gap-1">
                      {variant === "active" ? (
                        <>
                          <button
                            onClick={() => setEditingId(category.id)}
                            className="rounded p-1.5 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
                            aria-label="Edit category"
                          >
                            <Pencil className="h-3.5 w-3.5" />
                          </button>
                          <button
                            onClick={() => setDeactivatingId(category.id)}
                            className="rounded p-1.5 text-muted-foreground transition-colors hover:bg-destructive/10 hover:text-destructive"
                            aria-label="Deactivate category"
                          >
                            <Ban className="h-3.5 w-3.5" />
                          </button>
                        </>
                      ) : (
                        <button
                          onClick={() => restoreMutation.mutate(category.id)}
                          disabled={restoreMutation.isPending}
                          className="flex items-center gap-1 rounded px-2 py-1 text-xs font-medium text-muted-foreground transition-colors hover:bg-accent hover:text-foreground disabled:opacity-50"
                        >
                          <RotateCcw className="h-3.5 w-3.5" />
                          Restore
                        </button>
                      )}
                    </div>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {editingCategory && (
        <CategoryFormDialog
          mode="edit"
          workspaceId={workspaceId}
          category={editingCategory}
          open={editingId != null}
          onOpenChange={(open) => !open && setEditingId(null)}
        />
      )}

      {deactivatingId != null && (
        <DeactivateCategoryDialog
          open={deactivatingId != null}
          onOpenChange={(open) => !open && setDeactivatingId(null)}
          onConfirm={() => {
            deactivateMutation.mutate(deactivatingId, {
              onSuccess: () => setDeactivatingId(null),
            });
          }}
          isPending={deactivateMutation.isPending}
        />
      )}
    </>
  );
};

export default CategoryTable;
