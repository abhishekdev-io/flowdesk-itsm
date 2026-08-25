import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { AssetCategoryResponse } from "@/types/assetCategory.types";
import {
  useCreateAssetCategory,
  useUpdateAssetCategory,
} from "@/hook/useCategoryQueries";

const schema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters").max(50),
  prefix: z
    .string()
    .min(2, "Prefix must be at least 2 characters")
    .max(6, "Prefix must be 6 characters or fewer")
    .regex(/^[A-Z0-9]+$/, "Uppercase letters and numbers only"),
});

type FormValues = z.infer<typeof schema>;

interface CategoryFormDialogProps {
  workspaceId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  category?: AssetCategoryResponse;
}

const CategoryFormDialog = ({
  workspaceId,
  open,
  onOpenChange,
  mode,
  category,
}: CategoryFormDialogProps) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: category?.name ?? "",
      prefix: category?.prefix ?? "",
    },
  });

  useEffect(() => {
    if (open) {
      reset({ name: category?.name ?? "", prefix: category?.prefix ?? "" });
    }
  }, [open, category, reset]);

  const createMutation = useCreateAssetCategory(workspaceId);
  const updateMutation = useUpdateAssetCategory(
    workspaceId,
    category?.id ?? -1,
  );

  const mutation = mode === "create" ? createMutation : updateMutation;

  const onSubmit = (values: FormValues) => {
    mutation.mutate(values, {
      onSuccess: () => onOpenChange(false),
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {mode === "create" ? "New category" : "Edit category"}
          </DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-1.5">
            <label htmlFor="name" className="text-sm font-medium">
              Name
            </label>
            <input
              id="name"
              {...register("name")}
              className="w-full rounded-md border border-border bg-background px-3 py-2 text-sm"
              placeholder="Laptops"
            />
            {errors.name && (
              <p className="text-xs text-destructive">{errors.name.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <label htmlFor="prefix" className="text-sm font-medium">
              Prefix
            </label>
            <input
              id="prefix"
              {...register("prefix")}
              className="w-full rounded-md border border-border bg-background px-3 py-2 font-mono text-sm uppercase"
              placeholder="LAP"
            />
            {errors.prefix && (
              <p className="text-xs text-destructive">
                {errors.prefix.message}
              </p>
            )}
          </div>

          {mutation.isError && (
            <p className="text-xs text-destructive">
              {(mutation.error as Error)?.message ?? "Something went wrong."}
            </p>
          )}

          <div className="flex justify-end gap-2 pt-2">
            <button
              type="button"
              onClick={() => onOpenChange(false)}
              className="rounded-md px-3 py-1.5 text-sm text-muted-foreground hover:bg-accent"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={mutation.isPending}
              className="rounded-md bg-primary px-3 py-1.5 text-sm font-medium text-primary-foreground hover:bg-primary/90 disabled:opacity-50"
            >
              {mutation.isPending
                ? "Saving…"
                : mode === "create"
                  ? "Create"
                  : "Save"}
            </button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CategoryFormDialog;
