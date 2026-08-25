import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { getInitials, resolveAvatarUrl } from "@/hook/useAvatarUrl";
import { useRestoreTicketComment, useTicketDeletedComments } from "@/hook/useTicketQueries";
import { formatRelativeTime } from "@/lib/format-time";
import { RotateCcw } from "lucide-react";


const DeletedComments = ({
  workspaceId,
  ticketId,
}: {
  workspaceId: string;
  ticketId: string;
}) => {

    const { data: deleted, isLoading } = useTicketDeletedComments(
    workspaceId,
    ticketId,
  );
  const restoreComment = useRestoreTicketComment(workspaceId, ticketId);
 
  if (isLoading) {
    return <p className="text-xs text-muted-foreground mt-2">Loading…</p>;
  }
 
  if (!deleted || deleted.totalCount === 0) {
    return (
      <p className="text-xs text-muted-foreground mt-2">
        No deleted comments.
      </p>
    );
  }

  return (
    <div className="mt-3 space-y-3">
      {deleted?.items.map((comment) => (
        <div
          key={comment.id}
          className="flex gap-3 rounded-md border border-dashed border-border/60 p-2.5"
        >
          <Avatar className="h-7 w-7 shrink-0 opacity-60">
            <AvatarImage src={resolveAvatarUrl(comment.author.avatarUrl)} />
            <AvatarFallback className="text-xs">
              {getInitials(comment.author.username)}
            </AvatarFallback>
          </Avatar>
          <div className="flex-1 min-w-0 space-y-1">
            <div className="flex items-center justify-between gap-2">
              <div className="flex items-baseline gap-1.5 min-w-0 flex-wrap">
                <span className="text-xs font-medium truncate text-muted-foreground">
                  {comment.author.username}
                </span>
                <span className="text-[11px] text-muted-foreground shrink-0">
                  · deleted {formatRelativeTime(comment.deletedAt)}
                </span>
              </div>
              <Button
                variant="ghost"
                size="sm"
                className="h-6 gap-1 text-xs shrink-0"
                onClick={() => restoreComment.mutate(comment.id)}
                disabled={restoreComment.isPending}
              >
                <RotateCcw className="h-3 w-3" />
                Restore
              </Button>
            </div>
            <p className="text-sm text-muted-foreground whitespace-pre-wrap">
              {comment.description}
            </p>
          </div>
        </div>
      ))}
    </div>
  )
}

export default DeletedComments;