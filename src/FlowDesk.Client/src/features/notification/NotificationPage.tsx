import { useNavigate, useParams } from "react-router";
import Spinner from "@/components/Spinner";
import type { NotificationResponse } from "@/types/notification.types";
import {
  useDeleteNotifications,
  useMarkNotificationRead,
  useNotifications,
} from "@/hook/useNotificationQueries";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  ArrowUpCircle,
  Bell,
  CheckCheck,
  Inbox,
  MessageSquare,
  RefreshCw,
  Shield,
  Ticket,
  Trash2,
  UserMinus,
  UserPlus,
  Users,
} from "lucide-react";

const EVENT_CONFIG: Record<
  NotificationResponse["eventType"],
  { label: string; icon: typeof Bell; color: string }
> = {
  TicketCreated: { label: "New ticket", icon: Ticket, color: "text-blue-500" },
  TicketAssigned: {
    label: "Ticket assigned",
    icon: UserPlus,
    color: "text-emerald-500",
  },
  TicketUnassigned: {
    label: "Ticket unassigned",
    icon: UserMinus,
    color: "text-muted-foreground",
  },
  TicketStatusChanged: {
    label: "Status changed",
    icon: RefreshCw,
    color: "text-amber-500",
  },
  TicketCommentAdded: {
    label: "New comment",
    icon: MessageSquare,
    color: "text-violet-500",
  },
  TicketPriorityChanged: {
    label: "Priority changed",
    icon: ArrowUpCircle,
    color: "text-red-500",
  },
  AddedToWorkspace: {
    label: "Added to workspace",
    icon: Users,
    color: "text-blue-500",
  },
  RemovedFromWorkspace: {
    label: "Removed from workspace",
    icon: Users,
    color: "text-muted-foreground",
  },
  WorkspaceRoleChanged: {
    label: "Role changed",
    icon: Shield,
    color: "text-amber-500",
  },
};

const formatRelativeTime = (iso: string) => {
  const diffMs = Date.now() - new Date(iso).getTime();
  const diffMin = Math.floor(diffMs / 60000);
  const diffHr = Math.floor(diffMin / 60);
  const diffDay = Math.floor(diffHr / 24);

  if (diffMin < 1) return "just now";
  if (diffMin < 60) return `${diffMin}m ago`;
  if (diffHr < 24) return `${diffHr}h ago`;
  if (diffDay < 7) return `${diffDay}d ago`;
  return new Date(iso).toLocaleDateString(undefined, {
    month: "short",
    day: "numeric",
  });
};

const NotificationPage = () => {
  const { workspaceId } = useParams();
  const navigate = useNavigate();
  const { data, isLoading } = useNotifications(workspaceId!);
  const markRead = useMarkNotificationRead(workspaceId!);
  const deleteNotifications = useDeleteNotifications(workspaceId!);
  const [selected, setSelected] = useState<Set<string>>(new Set());

  if (isLoading) return <Spinner />;

  const items = data?.items ?? [];
  const unreadIds = items.filter((n) => !n.isRead).map((n) => n.id);
  const allSelected = items.length > 0 && selected.size === items.length;

  const toggleSelect = (id: string) => {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  };

  const toggleSelectAll = () => {
    setSelected(allSelected ? new Set() : new Set(items.map((n) => n.id)));
  };

  const handleOpen = (n: NotificationResponse) => {
    if (!n.isRead) markRead.mutate([n.id]);
    if (n.relatedTicketId)
      navigate(`/workspaces/${workspaceId}/tickets/${n.relatedTicketId}`);
  };

  return (
    <div className="p-4 max-w-3xl mx-auto space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <h1 className="text-2xl font-semibold tracking-tight">Notifications</h1>
          {unreadIds.length > 0 && (
            <span className="text-xs font-medium px-2 py-0.5 rounded-full bg-primary/10 text-primary">
              {unreadIds.length} unread
            </span>
          )}
        </div>
        <div className="flex gap-2">
          {unreadIds.length > 0 && (
            <Button
              variant="outline"
              size="sm"
              disabled={markRead.isPending}
              onClick={() => markRead.mutate(unreadIds)}
              className="gap-1.5"
            >
              <CheckCheck className="h-3.5 w-3.5" />
              Mark all read
            </Button>
          )}
          {selected.size > 0 && (
            <Button
              variant="destructive"
              size="sm"
              disabled={deleteNotifications.isPending}
              onClick={() => {
                deleteNotifications.mutate(Array.from(selected));
                setSelected(new Set());
              }}
              className="gap-1.5"
            >
              <Trash2 className="h-3.5 w-3.5" />
              Delete ({selected.size})
            </Button>
          )}
        </div>
      </div>

      {/* Select-all bar — only shows once there's something to select */}
      {items.length > 0 && (
        <div className="flex items-center gap-2 px-1">
          <input
            type="checkbox"
            checked={allSelected}
            onChange={toggleSelectAll}
            className="h-3.5 w-3.5 rounded border-border accent-primary cursor-pointer"
          />
          <button
            onClick={toggleSelectAll}
            className="text-xs text-muted-foreground hover:text-foreground transition-colors"
          >
            {allSelected ? "Deselect all" : "Select all"}
          </button>
        </div>
      )}

      {/* Empty state */}
      {items.length === 0 && (
        <div className="flex flex-col items-center justify-center py-16 text-center border border-dashed rounded-xl bg-card/30">
          <div className="flex h-12 w-12 items-center justify-center rounded-full bg-muted/60 mb-3 text-muted-foreground">
            <Inbox className="h-6 w-6" />
          </div>
          <p className="text-sm font-medium">You're all caught up</p>
          <p className="text-xs text-muted-foreground mt-1">
            New notifications will show up here.
          </p>
        </div>
      )}

      {/* Notification list */}
      <div className="space-y-2">
        {items.map((n) => {
          const config = EVENT_CONFIG[n.eventType];
          const Icon = config.icon;
          const isSelected = selected.has(n.id);

          return (
            <div
              key={n.id}
              className={`group relative p-3 rounded-lg border flex gap-3 items-start transition-all cursor-pointer
                ${n.isRead ? "border-border/60 bg-card/40" : "border-primary/30 bg-primary/3 shadow-2xs"}
                ${isSelected ? "ring-2 ring-primary/40 border-primary/40" : "hover:border-border hover:bg-accent/20"}
              `}
              onClick={() => handleOpen(n)}
            >
              <input
                type="checkbox"
                checked={isSelected}
                onChange={() => toggleSelect(n.id)}
                onClick={(e) => e.stopPropagation()}
                className="mt-1 h-3.5 w-3.5 rounded border-border accent-primary cursor-pointer shrink-0"
              />

              <div className={`mt-0.5 shrink-0 ${config.color}`}>
                <Icon className="h-4 w-4" />
              </div>

              <div className="flex-1 min-w-0">
                <div className="flex items-center justify-between gap-2">
                  <span className="text-xs font-medium text-muted-foreground">
                    {config.label}
                  </span>
                  <div className="flex items-center gap-2 shrink-0">
                    <span className="text-xs text-muted-foreground">
                      {formatRelativeTime(n.createdAt)}
                    </span>
                    {!n.isRead && (
                      <span className="h-1.5 w-1.5 rounded-full bg-primary" />
                    )}
                  </div>
                </div>
                <p
                  className={`text-sm mt-1 ${n.isRead ? "font-normal text-foreground/80" : "font-medium text-foreground"}`}
                >
                  {n.title}
                </p>
                <p className="text-sm text-muted-foreground line-clamp-2 mt-0.5">
                  {n.body}
                </p>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default NotificationPage;
