import { getInitials, resolveAvatarUrl } from "@/hook/useAvatarUrl";
import { useTicketActivity } from "@/hook/useTicketQueries";
import { formatRelativeTime } from "@/lib/format-time";
import { TicketActivityType, type TicketActivityResponse } from "@/types/ticket.types";
import type { WorkspaceRole } from "@/types/workspace.types";
import { Activity, AlertTriangle, Archive, ArchiveRestore, ArrowRightLeft, Clock, Edit3, Flame, Headphones, Layers, MessageSquare, PlusCircle, ShieldCheck, Sparkles, User, UserMinus, UserPlus } from "lucide-react";

interface TicketActivityLogProps {
  workspaceId: string;
  ticketId: string;
}

// Map Activity Enum / Action String to modern Jira-style visual tokens
const getActivityMeta = (action: TicketActivityResponse["action"]) => {
  const parsedAction = typeof action === "string" && !isNaN(Number(action)) 
    ? Number(action) 
    : action;

  switch (parsedAction) {
    case TicketActivityType.Created:
    case "Created":
      return {
        icon: PlusCircle,
        bgColor: "bg-emerald-500/10 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-400",
        border: "border-emerald-500/30",
      };

    case TicketActivityType.StatusChanged:
    case "StatusChanged":
      return {
        icon: ArrowRightLeft,
        bgColor: "bg-blue-500/10 dark:bg-blue-500/20 text-blue-600 dark:text-blue-400",
        border: "border-blue-500/30",
      };

    case TicketActivityType.Archived:
    case "Archived":
      return {
        icon: Archive,
        bgColor: "bg-zinc-500/10 dark:bg-zinc-500/20 text-zinc-600 dark:text-zinc-400",
        border: "border-zinc-500/30",
      };

    case TicketActivityType.UnArchived:
    case "UnArchived":
      return {
        icon: ArchiveRestore,
        bgColor: "bg-teal-500/10 dark:bg-teal-500/20 text-teal-600 dark:text-teal-400",
        border: "border-teal-500/30",
      };

    case TicketActivityType.Assigned:
    case "Assigned":
      return {
        icon: UserPlus,
        bgColor: "bg-purple-500/10 dark:bg-purple-500/20 text-purple-600 dark:text-purple-400",
        border: "border-purple-500/30",
      };

    case TicketActivityType.Unassigned:
    case "Unassigned":
      return {
        icon: UserMinus,
        bgColor: "bg-orange-500/10 dark:bg-orange-500/20 text-orange-600 dark:text-orange-400",
        border: "border-orange-500/30",
      };

    case TicketActivityType.Updated:
    case "Updated":
      return {
        icon: Edit3,
        bgColor: "bg-sky-500/10 dark:bg-sky-500/20 text-sky-600 dark:text-sky-400",
        border: "border-sky-500/30",
      };

    case TicketActivityType.PriorityChanged:
    case "PriorityChanged":
      return {
        icon: AlertTriangle,
        bgColor: "bg-amber-500/10 dark:bg-amber-500/20 text-amber-600 dark:text-amber-400",
        border: "border-amber-500/30",
      };

    case TicketActivityType.TypeChanged:
    case "TypeChanged":
      return {
        icon: Layers,
        bgColor: "bg-violet-500/10 dark:bg-violet-500/20 text-violet-600 dark:text-violet-400",
        border: "border-violet-500/30",
      };

    case TicketActivityType.SlaBreached:
    case "SlaBreached":
      return {
        icon: Flame,
        bgColor: "bg-rose-500/15 dark:bg-rose-500/25 text-rose-600 dark:text-rose-400",
        border: "border-rose-500/40",
      };

    case TicketActivityType.CommentAdded:
    case "CommentAdded":
      return {
        icon: MessageSquare,
        bgColor: "bg-indigo-500/10 dark:bg-indigo-500/20 text-indigo-600 dark:text-indigo-400",
        border: "border-indigo-500/30",
      };

    default:
      return {
        icon: Sparkles,
        bgColor: "bg-muted text-muted-foreground",
        border: "border-border",
      };
  }
};

// ITSM Role Tag Badges: member, agent, admin
const getRoleBadge = (role?: WorkspaceRole) => {
  const normalized = (role || "").toLowerCase();

  switch (normalized) {
    case "admin":
      return {
        label: "Admin",
        icon: ShieldCheck,
        style: "bg-red-500/10 text-red-700 dark:text-red-300 border-red-500/20",
      };
    case "agent":
      return {
        label: "Agent",
        icon: Headphones,
        style: "bg-blue-500/10 text-blue-700 dark:text-blue-300 border-blue-500/20",
      };
    case "member":
      return {
        label: "Member",
        icon: User,
        style: "bg-muted text-muted-foreground border-border",
      };
    default:
      return null;
  }
};

const TicketActivityLog = ({
  workspaceId,
  ticketId,
}: TicketActivityLogProps) => {
  const { data: activity, isLoading } = useTicketActivity(
    workspaceId,
    ticketId,
  );

  // activity?.map((entry) => (console.log(entry.occuredAt)
  // ));

  if (isLoading) {
    return (
      <div className="space-y-4 py-2 animate-pulse">
        {[1, 2, 3].map((i) => (
          <div key={i} className="flex gap-3 items-center">
            <div className="h-6 w-6 rounded-full bg-muted" />
            <div className="flex-1 space-y-1.5">
              <div className="h-3 w-1/3 bg-muted rounded" />
              <div className="h-2.5 w-1/4 bg-muted/60 rounded" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (!activity || activity.length === 0) {
    return (
      <div className="rounded-lg border border-dashed border-border/70 p-8 text-center bg-card/30">
        <Activity className="mx-auto h-7 w-7 text-muted-foreground/40 mb-2" />
        <p className="text-sm font-medium text-foreground">No activity recorded</p>
        <p className="text-xs text-muted-foreground mt-0.5">
          Audited field changes and lifecycle events will populate here.
        </p>
      </div>
    );
  }
  return (
    <div className="flow-root py-1">
      <ol className="relative ml-3 border-l border-border/70">
        {activity.map((entry) => {
          const meta = getActivityMeta(entry.action);
          const ActionIcon = meta.icon;
          const roleBadge = getRoleBadge(entry.performedBy?.role);
          const userInitial = getInitials(entry.performedBy?.username);

          return (
            <li
              key={entry.id}
              className="group mb-5 ml-6 last:mb-1 transition-all"
            >
              {/* Activity Node Icon */}
              <span
                className={`absolute -left-3 flex h-6 w-6 items-center justify-center rounded-full ring-4 ring-card border ${meta.bgColor} ${meta.border} transition-transform group-hover:scale-110`}
              >
                <ActionIcon className="h-3 w-3" />
              </span>

              {/* Event Container */}
              <div className="flex flex-col gap-1.5 rounded-md border border-transparent p-2 -my-1.5 transition-colors hover:border-border/60 hover:bg-muted/30">
                <div className="flex items-center gap-2 flex-wrap">
                  {/* Avatar */}
                  {entry.performedBy?.avatarUrl ? (
                    <img
                      src={resolveAvatarUrl(entry.performedBy.avatarUrl)}
                      alt={entry.performedBy.username}
                      className="h-5 w-5 rounded-full object-cover border border-border"
                    />
                  ) : (
                    <span className="flex h-5 w-5 items-center justify-center rounded-full bg-primary/10 text-[10px] font-bold text-primary border border-primary/20">
                      {userInitial}
                    </span>
                  )}

                  {/* Performed By Name */}
                  <span className="text-xs font-semibold text-foreground">
                    {entry.performedBy?.username}
                  </span>

                  {/* Role Tag */}
                  {roleBadge && (
                    <span
                      className={`inline-flex items-center gap-1 rounded-full border px-1.5 py-0.5 text-[9px] font-medium leading-none ${roleBadge.style}`}
                    >
                      <roleBadge.icon className="h-2.5 w-2.5" />
                      {roleBadge.label}
                    </span>
                  )}

                  {/* Relative Timestamp */}
                  <span className="ml-auto inline-flex items-center gap-1 text-[11px] text-muted-foreground/75 font-mono">
                    <Clock className="h-3 w-3" />
                    {formatRelativeTime(entry.occurredAt)}                    
                  </span>
                </div>

                {/* Audit Description */}
                <div className="pl-7 text-xs leading-relaxed text-muted-foreground/90">
                  {entry.description}
                </div>
              </div>
            </li>
          );
        })}
      </ol>
    </div>
  );
};

export default TicketActivityLog;
 