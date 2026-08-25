import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { useRemoveMember } from "@/hook/useWorkspaceQueries";
import { Trash2 } from "lucide-react";

interface RemoveMemberDialogProps {
  workspaceId: string;
  userId: string;
  username: string;
  disabled?: boolean;
}

const RemoveMemberDialog = ({
  workspaceId,
  userId,
  username,
  disabled,
}: RemoveMemberDialogProps) => {
  const { mutate, isPending } = useRemoveMember(workspaceId);
  return (
    <AlertDialog>
      <AlertDialogTrigger
        render={
          <Button
            variant="ghost"
            size="icon"
            disabled={disabled || isPending}
            className="h-8 w-8 text-muted-foreground hover:bg-destructive/10 hover:text-destructive"
          />
        }
      >
        <Trash2 className="h-4 w-4" />
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Remove {username}?</AlertDialogTitle>
          <AlertDialogDescription>
            They'll lose access to this workspace immediately. You can add them
            back later from the global user list.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction
            onClick={() => mutate(userId)}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            Remove
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
};

export default RemoveMemberDialog;
