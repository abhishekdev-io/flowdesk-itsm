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
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  useArchiveTicket,
  useCloseTicket,
  useReopenTicket,
  useResolveTicket,
  useUnArchiveTicket,
} from "@/hook/useTicketQueries";
import { ticketPermissions, type CurrentUser } from "@/lib/ticket-permissions";
import type { TicketDetailResponse } from "@/types/ticket.types";
import {
  Archive,
  ArchiveRestore,
  CheckCircle2,
  MoreHorizontal,
  RotateCcw,
} from "lucide-react";

interface TicketStatusActionsProps {
  workspaceId: string;
  ticket: TicketDetailResponse;
  currentUser: CurrentUser;
}

const TicketStatusActions = ({
  workspaceId,
  ticket,
  currentUser,
}: TicketStatusActionsProps) => {
  const perms = ticketPermissions(currentUser, ticket);

  const resolve = useResolveTicket(workspaceId, ticket.id);
  const close = useCloseTicket(workspaceId, ticket.id);
  const reopen = useReopenTicket(workspaceId, ticket.id);
  const archive = useArchiveTicket(workspaceId, ticket.id);
  const unarchive = useUnArchiveTicket(workspaceId, ticket.id);

  const hasPrimaryAction =
    perms.canResolve ||
    perms.canClose ||
    perms.canArchive ||
    perms.canUnarchive;

  if (!hasPrimaryAction && !perms.canReopen) return null;
  return (
    <div className="flex items-center gap-2">
      {perms.canResolve && (
        <Button
          size="sm"
          className="gap-1.5"
          onClick={() => resolve.mutate()}
          disabled={resolve.isPending}
        >
          <CheckCircle2 className="h-4 w-4" />
          {resolve.isPending ? "Resolving…" : "Mark resolved"}
        </Button>
      )}

      {perms.canClose && (
        <Button
          size="sm"
          className="gap-1.5"
          onClick={() => close.mutate()}
          disabled={close.isPending}
        >
          <CheckCircle2 className="h-4 w-4" />
          {close.isPending ? "Closing…" : "Close ticket"}
        </Button>
      )}

      {perms.canArchive && (
        <AlertDialog>
          <AlertDialogTrigger
            render={<Button size="sm" variant="outline" className="gap-1.5" />}
          >
            <Archive className="h-4 w-4" />
            Archive
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Archive this ticket?</AlertDialogTitle>
              <AlertDialogDescription>
                {ticket.ticketNumber} will be moved out of active ticket views.
                It can be unarchived later.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction onClick={() => archive.mutate()}>
                Archive
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {perms.canUnarchive && (
        <Button
          size="sm"
          variant="outline"
          className="gap-1.5"
          onClick={() => unarchive.mutate()}
          disabled={unarchive.isPending}
        >
          <ArchiveRestore className="h-4 w-4" />
          Unarchive
        </Button>
      )}

      {perms.canReopen && (
        <DropdownMenu>
          <DropdownMenuTrigger
            render={<Button size="sm" variant="ghost" className="px-2" />}
          >
            <MoreHorizontal className="h-4 w-4" />
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={() => reopen.mutate()}>
              <RotateCcw className="h-4 w-4 mr-2" />
              Reopen ticket
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      )}
    </div>
  );
};

export default TicketStatusActions;
