import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { useAssets } from "@/hook/useAssetQueries";
import { useCreateTicket } from "@/hook/useTicketQueries";

import { zodResolver } from "@hookform/resolvers/zod";
import { Controller, useForm } from "react-hook-form";
import { z } from "zod";

const createTicketSchema = z.object({
  title: z
    .string()
    .min(1, "Title is required")
    .max(200, "Title must be under 200 characters"),
  description: z
    .string()
    .min(1, "Description is required")
    .max(5000, "Description must be under 5000 characters"),
  assetId: z.string().optional(),
  type: z.enum(["Incident", "ServiceRequest", "Problem", "ChangeRequest"], {
    message: "Type is required",
  }),
  priority: z.enum(["Low", "Medium", "High", "Critical"], {
    message: "Priority is required",
  }),
});

type CreateTicketFormValues = z.infer<typeof createTicketSchema>;

const TYPE_OPTIONS: { value: CreateTicketFormValues["type"]; label: string }[] =
  [
    { value: "Incident", label: "Incident" },
    { value: "ServiceRequest", label: "Service Request" },
    { value: "Problem", label: "Problem" },
    { value: "ChangeRequest", label: "Change Request" },
  ];

const PRIORITY_OPTIONS: {
  value: CreateTicketFormValues["priority"];
  label: string;
}[] = [
  { value: "Low", label: "Low" },
  { value: "Medium", label: "Medium" },
  { value: "High", label: "High" },
  { value: "Critical", label: "Critical" },
];

interface CreateTicketDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  workspaceId: string;
}

const CreateTicketDialog = ({
  open,
  onOpenChange,
  workspaceId,
}: CreateTicketDialogProps) => {
  const { data: assetsData } = useAssets(workspaceId, {
    page: 1,
    pageSize: 100,
  });
  const assets = assetsData?.items ?? [];

  const { mutate, isPending } = useCreateTicket(workspaceId);

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateTicketFormValues>({
    resolver: zodResolver(createTicketSchema),
    defaultValues: {
      title: "",
      description: "",
      assetId: undefined,
      type: "Incident",
      priority: "Medium",
    },
  });

  const handleClose = (isOpen: boolean) => {
    if (!isOpen) reset();
    onOpenChange(isOpen);
  };

  const onSubmit = (values: CreateTicketFormValues) => {
    mutate(
      {
        ...values,
        assetId: values.assetId || undefined,
      },
      {
        onSuccess: () => handleClose(false),
      },
    );
  };
  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>New ticket</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Title */}
          <div className="space-y-1.5">
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              placeholder="Short summary of the issue"
              {...register("title")}
            />
            {errors.title && (
              <p className="text-xs text-destructive">{errors.title.message}</p>
            )}
          </div>

          {/* Description */}
          <div className="space-y-1.5">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              rows={4}
              placeholder="Describe the issue in detail..."
              {...register("description")}
            />
            {errors.description && (
              <p className="text-xs text-destructive">
                {errors.description.message}
              </p>
            )}
          </div>

          <div className="grid grid-cols-2 gap-3">
            {/* Type */}
            <div className="space-y-1.5">
              <Label>Type</Label>
              <Controller
                control={control}
                name="type"
                render={({ field }) => (
                  <Select
                    value={field.value}
                    onValueChange={(value) => value && field.onChange(value)}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Select type" />
                    </SelectTrigger>
                    <SelectContent>
                      {TYPE_OPTIONS.map((opt) => (
                        <SelectItem key={opt.value} value={opt.value}>
                          {opt.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
              {errors.type && (
                <p className="text-xs text-destructive">
                  {errors.type.message}
                </p>
              )}
            </div>

            {/* Priority */}
            <div className="space-y-1.5">
              <Label>Priority</Label>
              <Controller
                control={control}
                name="priority"
                render={({ field }) => (
                  <Select
                    value={field.value}
                    onValueChange={(value) => value && field.onChange(value)}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Select priority" />
                    </SelectTrigger>
                    <SelectContent>
                      {PRIORITY_OPTIONS.map((opt) => (
                        <SelectItem key={opt.value} value={opt.value}>
                          {opt.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
              {errors.priority && (
                <p className="text-xs text-destructive">
                  {errors.priority.message}
                </p>
              )}
            </div>
          </div>

          {/* Asset */}
          <div className="space-y-1.5">
            <Label>Asset</Label>
            <Controller
              control={control}
              name="assetId"
              render={({ field }) => {
                const activeAssets = assets.filter(
                  (asset) => asset.status !== "Decommissioned",
                );
                const selectedAsset = activeAssets.find(
                  (a) => a.id === field.value,
                );

                return (
                  <Select
                    value={field.value ?? "none"}
                    onValueChange={(value) =>
                      field.onChange(value === "none" ? undefined : value)
                    }
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="No asset linked">
                        {selectedAsset ? selectedAsset.name : "No asset linked"}
                      </SelectValue>
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="none">No asset</SelectItem>
                      {activeAssets.map((asset) => (
                        <SelectItem key={asset.id} value={asset.id}>
                          {asset.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                );
              }}
            />
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => handleClose(false)}
              disabled={isPending}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? "Creating..." : "Create ticket"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CreateTicketDialog;
