import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface DeactivateCategoryDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: () => void;
  isPending: boolean;
}

const DeactivateCategoryDialog = ({
  open,
  onOpenChange,
  onConfirm,
  isPending,
}: DeactivateCategoryDialogProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Deactivate category?</DialogTitle>
        </DialogHeader>
        <p className="text-sm text-muted-foreground">
          Assets already tagged with this category keep their tag, but it won't
          be selectable for new assets until restored.
        </p>
        <div className="flex justify-end gap-2 pt-4">
          <button
            onClick={() => onOpenChange(false)}
            className="rounded-md px-3 py-1.5 text-sm text-muted-foreground hover:bg-accent"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            disabled={isPending}
            className="rounded-md bg-destructive px-3 py-1.5 text-sm font-medium text-destructive-foreground hover:bg-destructive/90 disabled:opacity-50"
          >
            {isPending ? "Deactivating…" : "Deactivate"}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default DeactivateCategoryDialog;
