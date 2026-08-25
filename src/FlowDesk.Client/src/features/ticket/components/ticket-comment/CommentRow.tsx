import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Textarea } from "@/components/ui/textarea";
import { getInitials, resolveAvatarUrl } from "@/hook/useAvatarUrl";
import {
  useDeleteTicketComment,
  useUpdateTicketComment,
} from "@/hook/useTicketQueries";
import { formatRelativeTime } from "@/lib/format-time";
import type { TicketCommentResponse } from "@/types/ticket.types";
import { MoreHorizontal } from "lucide-react";
import { useState } from "react";

interface CommentRowProps {
  workspaceId: string;
  ticketId: string;
  comment: TicketCommentResponse & { author: { id?: string } };
  currentUserId: string;
  isAdmin: boolean;
}

const CommentRow = ({
  workspaceId,
  ticketId,
  comment,
  currentUserId,
  isAdmin,
}: CommentRowProps) => {
  const [isEditing, setIsEditing] = useState(false);
  const [value, setValue] = useState(comment.description);

  const updateComment = useUpdateTicketComment(workspaceId, ticketId);
  const deleteComment = useDeleteTicketComment(workspaceId, ticketId);

  const isOwn = comment.author.id === currentUserId;
  const canEdit = isOwn;
  const canDelete = isOwn || isAdmin;

  const handleSave = () => {
    if (!value.trim()) return;
    updateComment.mutate(
      { commentId: comment.id, payload: { description: value.trim() } },
      { onSuccess: () => setIsEditing(false) },
    );
  };

  return (
    <div className="flex gap-3">
      <Avatar className="h-8 w-8 shrink-0">
        <AvatarImage src={resolveAvatarUrl(comment.author.avatarUrl)} />
        <AvatarFallback className="text-xs">
          {getInitials(comment.author.username)}
        </AvatarFallback>
      </Avatar>

      <div className="flex-1 min-w-0 space-y-1">
        <div className="flex items-center justify-between gap-2">
          <div className="flex items-baseline gap-1.5 min-w-0 flex-wrap">
            <span className="text-sm font-medium truncate">
              {comment.author.username}
            </span>
            <span className="text-[11px] text-muted-foreground shrink-0">
              {comment.author.role}
            </span>
            <span className="text-[11px] text-muted-foreground shrink-0">
              · {formatRelativeTime(comment.createdAt)}
            </span>
            {comment.isEdited && (
              <span className="text-[11px] text-muted-foreground shrink-0">
                (edited)
              </span>
            )}
          </div>

          {(canEdit || canDelete) && !isEditing && (
            <DropdownMenu>
              <DropdownMenuTrigger
                render={
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 shrink-0"
                  />
                }
              >
                <MoreHorizontal className="h-3.5 w-3.5" />
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                {canEdit && (
                  <DropdownMenuItem onClick={() => setIsEditing(true)}>
                    Edit
                  </DropdownMenuItem>
                )}
                {canDelete && (
                  <DropdownMenuItem
                    className="text-destructive focus:text-destructive"
                    onClick={() => deleteComment.mutate(comment.id)}
                  >
                    Delete
                  </DropdownMenuItem>
                )}
              </DropdownMenuContent>
            </DropdownMenu>
          )}
        </div>

        {isEditing ? (
          <div className="space-y-2">
            <Textarea
              value={value}
              onChange={(e) => setValue(e.target.value)}
              rows={3}
              autoFocus
            />
            <div className="flex justify-end gap-2">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => {
                  setValue(comment.description);
                  setIsEditing(false);
                }}
              >
                Cancel
              </Button>
              <Button
                size="sm"
                onClick={handleSave}
                disabled={!value.trim() || updateComment.isPending}
              >
                {updateComment.isPending ? "Saving…" : "Save"}
              </Button>
            </div>
          </div>
        ) : (
          <p className="text-sm text-foreground/90 whitespace-pre-wrap">
            {comment.description}
          </p>
        )}
      </div>
    </div>
  );
};

export default CommentRow;
