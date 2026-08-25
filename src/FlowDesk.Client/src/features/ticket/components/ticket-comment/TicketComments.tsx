import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { useCreateTicketComment, useTicketComments } from "@/hook/useTicketQueries";
import { MessageSquare } from "lucide-react";
import { useState } from "react";
import CommentRow from "./CommentRow";
import DeletedComments from "./DeletedComments";

 
interface TicketCommentsProps {
  workspaceId: string;
  ticketId: string;
  currentUserId: string;
  isAdmin: boolean;
  canComment: boolean;
}

const TicketComments = ({
  workspaceId,
  ticketId,
  currentUserId,
  isAdmin,
  canComment,
}: TicketCommentsProps) => {

    const { data: comments, isLoading } = useTicketComments(
    workspaceId,
    ticketId,
  );
  const createComment = useCreateTicketComment(workspaceId, ticketId);
  const [draft, setDraft] = useState("");
  const [showDeleted, setShowDeleted] = useState(false);
 
  const handlePost = () => {
    if (!draft.trim()) return;
    createComment.mutate(
      { description: draft.trim() },
      { onSuccess: () => setDraft("") },
    );
  };
  return (
    <div className="space-y-4">
      {canComment && (
        <div className="space-y-2">
          <Textarea
            placeholder="Add a comment…"
            rows={3}
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
          />
          <div className="flex justify-end">
            <Button
              size="sm"
              onClick={handlePost}
              disabled={!draft.trim() || createComment.isPending}
            >
              {createComment.isPending ? "Posting…" : "Comment"}
            </Button>
          </div>
        </div>
      )}
 
      {isLoading && (
        <p className="text-sm text-muted-foreground">Loading comments…</p>
      )}
 
      {!isLoading && comments?.totalCount === 0 && (
        <div className="text-sm text-muted-foreground flex items-center gap-2 py-4">
          <MessageSquare className="h-4 w-4" />
          No comments yet.
        </div>
      )}
 
      <div className="space-y-4">
        {comments?.items.map((comment) => (
          <CommentRow
            key={comment.id}
            workspaceId={workspaceId}
            ticketId={ticketId}
            comment={comment}
            currentUserId={currentUserId}
            isAdmin={isAdmin}
          />
        ))}
      </div>
 
      {isAdmin && (
        <div className="pt-2 border-t border-border/60">
          <Button
            variant="link"
            size="sm"
            className="h-auto p-0 text-xs text-muted-foreground"
            onClick={() => setShowDeleted((prev) => !prev)}
          >
            {showDeleted ? "Hide deleted comments" : "View deleted comments"}
          </Button>
          {showDeleted && (
            <DeletedComments workspaceId={workspaceId} ticketId={ticketId} />
          )}
        </div>
      )}
    </div>
  )
}

export default TicketComments;