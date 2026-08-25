import { useState } from "react";
import { useNavigate, useParams } from "react-router";
import { Button } from "@/components/ui/button";
import {
  Dialog, DialogContent, DialogHeader, DialogTitle,
  DialogDescription, DialogFooter,
} from "@/components/ui/dialog";
import { useDeleteWorkspace } from "@/hook/useWorkspaceQueries";

const DangerZoneSettingsPage = () => {
  const { workspaceId } = useParams<{ workspaceId: string }>();
  const navigate = useNavigate();
  const [confirmOpen, setConfirmOpen] = useState(false);

  const { mutate: deleteWorkspace, isPending } = useDeleteWorkspace(workspaceId!);

  const handleDelete = () => {
    deleteWorkspace(undefined, {
      onSuccess: () => {
        setConfirmOpen(false);
        navigate("/hub", { replace: true });
      },
    });
  };
  return (
    <div className="mx-auto max-w-4xl p-6">
      <h1 className="mb-1 text-lg font-semibold">Danger zone</h1>
      <p className="mb-6 text-sm text-muted-foreground">
        Critical workspace actions and deletion settings.
      </p>

      <section className="space-y-3 rounded-md border border-destructive/40 p-4">
        <h2 className="text-sm font-medium text-destructive">Delete workspace</h2>
        <p className="text-sm text-muted-foreground">
          Deleting this workspace will immediately revoke access for all members and suspend all active operations. Data is retained and can be recovered by contacting your Flowdesk system administrator.
        </p>
        <Button variant="destructive" size="sm" onClick={() => setConfirmOpen(true)}>
          Delete workspace
        </Button>
      </section>

      <Dialog open={confirmOpen} onOpenChange={setConfirmOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Are you sure you want to delete this workspace?</DialogTitle>
            <DialogDescription className="space-y-2 pt-2 text-sm text-muted-foreground">
              <span>
                This workspace will be archived and inaccessible to all members.
              </span>
              <span className="block text-xs text-muted-foreground/80">
                If this was done in error or you need to restore tickets, assets, and settings, please reach out to a Flowdesk system administrator.
              </span>
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="gap-2 sm:gap-0">
            <Button
              variant="ghost"
              onClick={() => setConfirmOpen(false)}
              disabled={isPending}
            >
              Cancel
            </Button>
            <Button
              variant="destructive"
              disabled={isPending}
              onClick={handleDelete}
            >
              {isPending ? "Deleting…" : "Delete workspace"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}

export default DangerZoneSettingsPage;