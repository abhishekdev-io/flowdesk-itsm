import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import {
  createWorkspaceSchema,
  type CreateWorkspaceValues,
} from "@/types/workspace.types";
import { useCreateWorkspace } from "@/hook/useWorkspaceQueries";

interface CreateWorkspaceDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const CreateWorkspaceDialog = ({
  open,
  onOpenChange,
}: CreateWorkspaceDialogProps) => {
  const [serverError, setServerError] = useState<string | null>(null);

  const createWorkspaceMutation = useCreateWorkspace();

  const form = useForm<CreateWorkspaceValues>({
    resolver: zodResolver(createWorkspaceSchema),
    defaultValues: { Name: "", Description: "" },
  });

  const { isSubmitting } = form.formState;
  const isLoading = isSubmitting || createWorkspaceMutation.isPending;

  const onSubmit = async (values: CreateWorkspaceValues) => {
    setServerError(null);
    try {
      await createWorkspaceMutation.mutateAsync(values);

      form.reset();
      onOpenChange(false);
    } catch (err) {
      setServerError(
        err instanceof Error ? err.message : "Something went wrong.",
      );
    }
  };

  const handleOpenChange = (
    next: boolean,
    eventDetails?: { reason?: string; cancel: () => void },
  ) => {
    if (isLoading) {
      eventDetails?.cancel();
      return;
    }
    if (!next) {
      form.reset();
      setServerError(null);
    }
    onOpenChange(next);
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader className="space-y-1.5">
          <DialogTitle className="text-lg"> New workspace </DialogTitle>
          <DialogDescription className="text-sm">
            A workspace groups tickets, assets, and members under one board.
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="space-y-4"
          noValidate
        >
          {/* Workspace Name */}
          <Controller
            name="Name"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor="workspace-name">Workspace name</FieldLabel>
                <Input
                  {...field}
                  id="workspace-name"
                  aria-invalid={fieldState.invalid}
                  placeholder="Platform Engineering"
                  autoComplete="off"
                />
                {fieldState.invalid && (
                  <FieldError errors={[fieldState.error]} />
                )}
              </Field>
            )}
          />
          {/* Description */}
          <Controller
            name="Description"
            control={form.control}
            render={({ field, fieldState }) => (
              <Field data-invalid={fieldState.invalid}>
                <FieldLabel htmlFor="workspace-description">
                  Description
                </FieldLabel>
                <Textarea
                  {...field}
                  id="workspace-description"
                  aria-invalid={fieldState.invalid}
                  placeholder="What does this workspace cover?"
                  rows={3}
                  className="resize-none"
                />
                {fieldState.invalid && (
                  <FieldError errors={[fieldState.error]} />
                )}
              </Field>
            )}
          />
          {/* Server Error */}
          {serverError && (
            <div
              role="alert"
              className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive"
            >
              {serverError}
            </div>
          )}
          {/* Actions */}
          <DialogFooter className="gap-2 sm:gap-2">
            <Button
              type="button"
              variant="ghost"
              onClick={() => handleOpenChange(false)}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Creating..." : "Create workspace"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default CreateWorkspaceDialog;
