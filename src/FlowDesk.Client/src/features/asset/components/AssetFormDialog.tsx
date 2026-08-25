import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import Spinner from "@/components/Spinner";
import { useAssetCategories } from "@/hook/useCategoryQueries";
import { useAsset, useCreateAsset, useUpdateAsset } from "@/hook/useAssetQueries";

const schema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters").max(100),
  categoryId: z.number({ error: "Select a category" }).min(1),
  manufacturer: z.string().min(1, "Manufacturer is required").max(100),
  modelnumber: z.string().min(1, "Model number is required").max(100),
  serialnumber: z.string().min(1, "Serial number is required").max(100),
});

type FormValues = z.infer<typeof schema>;

interface AssetFormDialogProps {
  workspaceId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  assetId?: string; // only needed for edit mode
}

const EMPTY_VALUES: FormValues = {
  name: "",
  categoryId: 0,
  manufacturer: "",
  modelnumber: "",
  serialnumber: "",
};

const AssetFormDialog = ({
  workspaceId,
  open,
  onOpenChange,
  mode,
  assetId,
}: AssetFormDialogProps) => {
  const { data: categories } = useAssetCategories(workspaceId);

  const {
    data: assetDetail,
    isLoading: assetLoading,
    isError: assetError,
  } = useAsset(workspaceId, mode === "edit" && open ? (assetId ?? "") : "");

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    control,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: EMPTY_VALUES,
  });

  const categoryId = useWatch({ control, name: "categoryId" });

  // Reset the form once detail data actually arrives (edit mode), or
  // immediately for create mode — not before, or you'd flash stale values
  // from a previously-edited asset.
  useEffect(() => {
    if (!open) return;

    if (mode === "create") {
      reset(EMPTY_VALUES);
      return;
    }
    if (assetDetail) {
      reset({
        name: assetDetail.name,
        // category id isn't on AssetDetailResponse (only `category` name is) —
        // resolve it by matching name against the categories list until the
        // backend/type is updated to return categoryId directly.
        categoryId:
          categories?.find((c) => c.name === assetDetail.category)?.id ?? 0,
        manufacturer: assetDetail.manufacturer,
        modelnumber: assetDetail.modelnumber,
        serialnumber: assetDetail.serialnumber,
      });
    }
  }, [open, mode, assetDetail, categories, reset]);

  const createMutation = useCreateAsset(workspaceId);
  const updateMutation = useUpdateAsset(workspaceId, assetId ?? "");
  const mutation = mode === "create" ? createMutation : updateMutation;

  const onSubmit = (values: FormValues) => {
    mutation.mutate(values, { onSuccess: () => onOpenChange(false) });
  };

  const showLoadingState = mode === "edit" && open && assetLoading;
  const showErrorState = mode === "edit" && open && assetError;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {mode === "create" ? "New asset" : "Edit asset"}
          </DialogTitle>
        </DialogHeader>

        {showLoadingState ? (
          <div className="flex h-40 items-center justify-center">
            <Spinner />
          </div>
        ) : showErrorState ? (
          <p className="py-8 text-center text-sm text-destructive">
            Couldn't load this asset. Try closing and reopening.
          </p>
        ) : (
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-1.5">
              <label htmlFor="name" className="text-sm font-medium">
                Name
              </label>
              <Input
                id="name"
                {...register("name")}
                placeholder="Dell Latitude 5420"
              />
              {errors.name && (
                <p className="text-xs text-destructive">
                  {errors.name.message}
                </p>
              )}
            </div>

            <div className="space-y-1.5">
              <label className="text-sm font-medium">Category</label>
              <Select
                value={categoryId ? String(categoryId) : ""}
                onValueChange={(value) =>
                  value &&
                  setValue("categoryId", Number(value), {
                    shouldValidate: true,
                  })
                }
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Select category">
                    {categories?.find((c) => c.id === categoryId)?.name}
                  </SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {categories?.map((c) => (
                    <SelectItem key={c.id} value={String(c.id)}>
                      {c.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {errors.categoryId && (
                <p className="text-xs text-destructive">
                  {errors.categoryId.message}
                </p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1.5">
                <label htmlFor="manufacturer" className="text-sm font-medium">
                  Manufacturer
                </label>
                <Input
                  id="manufacturer"
                  {...register("manufacturer")}
                  placeholder="Dell"
                />
                {errors.manufacturer && (
                  <p className="text-xs text-destructive">
                    {errors.manufacturer.message}
                  </p>
                )}
              </div>
              <div className="space-y-1.5">
                <label htmlFor="modelnumber" className="text-sm font-medium">
                  Model number
                </label>
                <Input
                  id="modelnumber"
                  {...register("modelnumber")}
                  placeholder="5420"
                />
                {errors.modelnumber && (
                  <p className="text-xs text-destructive">
                    {errors.modelnumber.message}
                  </p>
                )}
              </div>
            </div>

            <div className="space-y-1.5">
              <label htmlFor="serialnumber" className="text-sm font-medium">
                Serial number
              </label>
              <Input
                id="serialnumber"
                {...register("serialnumber")}
                placeholder="SN-2026-0001"
              />
              {errors.serialnumber && (
                <p className="text-xs text-destructive">
                  {errors.serialnumber.message}
                </p>
              )}
            </div>

            {mutation.isError && (
              <p className="text-xs text-destructive">
                {(mutation.error as Error)?.message ?? "Something went wrong."}
              </p>
            )}

            <div className="flex justify-end gap-2 pt-2">
              <Button
                type="button"
                variant="ghost"
                onClick={() => onOpenChange(false)}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending
                  ? "Saving…"
                  : mode === "create"
                    ? "Create"
                    : "Save"}
              </Button>
            </div>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default AssetFormDialog;
