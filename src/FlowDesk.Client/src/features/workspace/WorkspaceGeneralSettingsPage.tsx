import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { getInitials } from "@/hook/useAvatarUrl";
import { useUpdateWorkspace, useWorkspace } from "@/hook/useWorkspaceQueries";
import type { UpdateWorkspaceRequest } from "@/types/workspace.types";
import { AlertCircle, Loader2 } from "lucide-react";
import { useEffect } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useParams } from "react-router";

const DESCRIPTION_MAX_LENGTH = 600;

const WorkspaceGeneralSettingsPage = () => {
  const { workspaceId } = useParams<{ workspaceId: string }>();
  const { data: workspace, isLoading } = useWorkspace(workspaceId!);
  const { mutate: updateWorkspace, isPending } = useUpdateWorkspace(
    workspaceId!,
  );

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { isDirty, errors },
  } = useForm<UpdateWorkspaceRequest>({
    defaultValues: {
      name: "",
      description: "",
    },
  });

  const nameValue = useWatch({ control, name: "name" });
  const descriptionValue = useWatch({ control, name: "description" }) ?? "";

  useEffect(() => {
    if (workspace) {
      reset({
        name: workspace.name,
        description: workspace.description ?? "",
      });
    }
  }, [workspace, reset]);

  const onSubmit = (data: UpdateWorkspaceRequest) => {
    updateWorkspace(data, {
      onSuccess: () => {
        reset(data); // resets the form's dirty state
      },
    });
  };

  const handleDiscard = () => {
    if (workspace) {
      reset({
        name: workspace.name,
        description: workspace.description ?? "",
      });
    }
  };

  if (isLoading) {
    return (
      <div className="p-6 max-w-2xl space-y-6">
        <div className="space-y-2">
          <div className="h-6 w-48 rounded bg-muted animate-pulse" />
          <div className="h-4 w-72 rounded bg-muted animate-pulse" />
        </div>
        <div className="rounded-lg border border-border/60 bg-card p-5 space-y-5">
          <div className="h-16 w-full rounded bg-muted animate-pulse" />
          <div className="h-24 w-full rounded bg-muted animate-pulse" />
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 max-w-2xl space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold tracking-tight">
            General settings
          </h1>
          <p className="text-sm text-muted-foreground">
            Manage your workspace details and branding.
          </p>
        </div>
        {isDirty && (
          <span className="mt-1 shrink-0 rounded-full bg-amber-500/10 px-2.5 py-1 text-xs font-medium text-amber-600 dark:text-amber-400">
            Unsaved changes
          </span>
        )}
      </div>

      <div className="rounded-lg border border-border/60 bg-card p-5">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          <div className="flex items-center gap-4">
            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-lg font-semibold text-primary">
              {getInitials(nameValue || workspace?.name || "?")}
            </div>
            <div className="flex-1 space-y-1.5">
              <Label htmlFor="name" className="text-xs font-medium">
                Workspace name
              </Label>
              <Input
                id="name"
                {...register("name", {
                  required: "Workspace name is required",
                })}
                placeholder="e.g. Acme Corp Operations"
                disabled={isPending}
                aria-invalid={!!errors.name}
              />
              {errors.name && (
                <p className="flex items-center gap-1 text-xs text-destructive">
                  <AlertCircle className="h-3 w-3" />
                  {errors.name.message}
                </p>
              )}
            </div>
          </div>

          <div className="space-y-1.5">
            <div className="flex items-center justify-between">
              <Label htmlFor="description" className="text-xs font-medium">
                Description
              </Label>
              <span className="text-xs text-muted-foreground">
                {descriptionValue.length}/{DESCRIPTION_MAX_LENGTH}
              </span>
            </div>
            <Textarea
              id="description"
              rows={3}
              {...register("description", {
                maxLength: {
                  value: DESCRIPTION_MAX_LENGTH,
                  message: `Description can't exceed ${DESCRIPTION_MAX_LENGTH} characters`,
                },
              })}
              placeholder="Describe what this workspace is used for..."
              disabled={isPending}
              aria-invalid={!!errors.description}
            />
            {errors.description && (
              <p className="flex items-center gap-1 text-xs text-destructive">
                <AlertCircle className="h-3 w-3" />
                {errors.description.message}
              </p>
            )}
          </div>

          <div className="flex justify-end gap-2 border-t border-border/60 pt-4">
            {isDirty && (
              <Button
                type="button"
                variant="ghost"
                size="sm"
                onClick={handleDiscard}
                disabled={isPending}
              >
                Discard
              </Button>
            )}
            <Button
              type="submit"
              size="sm"
              disabled={!isDirty || isPending}
              className="gap-1.5"
            >
              {isPending && <Loader2 className="h-3.5 w-3.5 animate-spin" />}
              Save changes
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default WorkspaceGeneralSettingsPage;
