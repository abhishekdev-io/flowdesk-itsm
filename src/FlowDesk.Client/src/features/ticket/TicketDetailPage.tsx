import Spinner from "@/components/Spinner";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useTicket } from "@/hook/useTicketQueries";
import { useAuth } from "@/stores/useAuth";
import { useMyWorkspaceRole } from "@/stores/useWorkspaceContext";
import type { TicketDetailResponse } from "@/types/ticket.types";
import {
  AlertCircle,
  ArrowLeft,
  Box,
  Clock,
  TicketIcon,
  User,
} from "lucide-react";
import { useNavigate, useParams } from "react-router";
import EditTicketDialog from "./components/EditTicketDialog";
import {
  TicketPriorityMenu,
  TicketTypeMenu,
} from "./components/TicketFieldMenus";
import TicketStatusActions from "./components/TicketStatusActions";
import { ticketPermissions, type CurrentUser } from "@/lib/ticket-permissions";
import { formatDateTime } from "@/lib/format-time";
import TicketActivityLog from "./components/ticket-activities/TicketActivityLog";
import TicketComments from "./components/ticket-comment/TicketComments";
import TicketAssignMenu from "./components/TicketAssignMenu";

const RISK_STYLES: Record<TicketDetailResponse["riskLevel"], string> = {
  NoSla: "text-muted-foreground bg-muted",
  OnTrack:
    "text-green-700 bg-green-50 dark:text-green-300 dark:bg-green-950/40",
  AtRisk: "text-amber-700 bg-amber-50 dark:text-amber-300 dark:bg-amber-950/40",
  Critical: "text-red-700 bg-red-50 dark:text-red-300 dark:bg-red-950/40",
  Breached: "text-red-800 bg-red-100 dark:text-red-200 dark:bg-red-950/60",
};

const STATUS_STYLES: Record<TicketDetailResponse["status"], string> = {
  Open: "text-blue-700 bg-blue-50 dark:text-blue-300 dark:bg-blue-950/40",
  InProgress:
    "text-amber-700 bg-amber-50 dark:text-amber-300 dark:bg-amber-950/40",
  Resolved:
    "text-green-700 bg-green-50 dark:text-green-300 dark:bg-green-950/40",
  Closed: "text-muted-foreground bg-muted",
};

const TicketDetailPage = () => {
  const { workspaceId, ticketId } = useParams();
  const navigate = useNavigate();
  const { data: ticket, isLoading } = useTicket(workspaceId!, ticketId!);

  const auth = useAuth.getState().auth;
  const user = auth.status === "authenticated" ? auth.user : null;

  const role = useMyWorkspaceRole();

  if (isLoading) return <Spinner />;

  if (!ticket) {
    return (
      <div className="p-6 max-w-4xl mx-auto text-center py-16">
        <p className="text-sm font-medium">Ticket not found</p>
        <Button
          variant="outline"
          size="sm"
          className="mt-4 gap-1"
          onClick={() => navigate(`/workspaces/${workspaceId}/tickets`)}
        >
          <ArrowLeft className="h-4 w-4" />
          Back to tickets
        </Button>
      </div>
    );
  }

  if (!user?.userId || !role) {
    return null;
  }

  const currentUser: CurrentUser = { id: user?.userId, role };
  const perms = ticketPermissions(currentUser, ticket);

  return (
    <div className="p-6 max-w-4xl mx-auto space-y-6">
      {/* Header */}
      <div className="space-y-3">
        <Button
          variant="ghost"
          size="sm"
          className="gap-1 -ml-2 text-muted-foreground"
          onClick={() => navigate(`/workspaces/${workspaceId}/tickets`)}
        >
          <ArrowLeft className="h-4 w-4" />
          Back to tickets
        </Button>

        <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-3">
          <div className="space-y-1 min-w-0">
            <div className="flex items-center gap-2 flex-wrap">
              <span className="font-mono text-xs font-semibold text-primary">
                {ticket.ticketNumber}
              </span>
              {ticket.archivedAt && (
                <span className="rounded bg-muted px-1.5 py-0.5 text-[10px] font-semibold text-muted-foreground border border-border">
                  Archived
                </span>
              )}
            </div>
            <div className="flex items-center gap-1.5">
              <h1 className="text-2xl font-semibold tracking-tight">
                {ticket.title}
              </h1>
              {perms.canEdit && (
                <EditTicketDialog workspaceId={workspaceId!} ticket={ticket} />
              )}
            </div>
          </div>

          <div className="flex items-center gap-2 shrink-0 flex-wrap justify-end">
            <span
              className={`text-xs px-2.5 py-1 rounded-md font-medium ${STATUS_STYLES[ticket.status]}`}
            >
              {ticket.status}
            </span>
            <TicketPriorityMenu
              workspaceId={workspaceId!}
              ticket={ticket}
              editable={perms.canChangePriority}
            />
            {ticket.riskLevel !== "NoSla" && (
              <span
                className={`text-xs px-2.5 py-1 rounded-md font-medium ${RISK_STYLES[ticket.riskLevel]}`}
              >
                {ticket.riskLevel}
              </span>
            )}
          </div>
        </div>

        <div className="flex justify-end">
          <TicketStatusActions
            workspaceId={workspaceId!}
            ticket={ticket}
            currentUser={currentUser}
          />
        </div>
      </div>

      {/* Description */}
      <div className="rounded-lg border border-border/60 bg-card p-4 space-y-2">
        <h2 className="text-sm font-medium text-foreground/80 flex items-center gap-1.5">
          <TicketIcon className="h-4 w-4 text-muted-foreground" />
          Description
        </h2>
        <p className="text-sm text-muted-foreground whitespace-pre-wrap">
          {ticket.description}
        </p>
      </div>

      {/* Meta grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div className="rounded-lg border border-border/60 bg-card p-4 space-y-3">
          <h2 className="text-sm font-medium text-foreground/80">People</h2>
          <div className="flex items-center gap-2 text-sm">
            <User className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Created by</span>
            <span className="font-medium">{ticket.createdByUser.username}</span>
          </div>
          <div className="flex items-center gap-2 text-sm">
            <User className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Assigned to</span>
            {/* Dynamic Assign/Unassign menu */}
            <TicketAssignMenu
              workspaceId={workspaceId!}
              ticket={ticket}
              canAssign={perms.canAssign}
              canUnassign={perms.canUnassign}
            />
          </div>
        </div>

        <div className="rounded-lg border border-border/60 bg-card p-4 space-y-3">
          <h2 className="text-sm font-medium text-foreground/80">Details</h2>
          <div className="flex items-center gap-2 text-sm">
            <AlertCircle className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Type</span>
            <TicketTypeMenu
              workspaceId={workspaceId!}
              ticket={ticket}
              editable={perms.canChangeType}
            />
          </div>
          {ticket.assetName && (
            <div className="flex items-center gap-2 text-sm">
              <Box className="h-4 w-4 text-muted-foreground shrink-0" />
              <span className="text-muted-foreground">Asset</span>
              <span className="font-medium text-primary/90">
                {ticket.assetName}
              </span>
            </div>
          )}
        </div>

        <div className="rounded-lg border border-border/60 bg-card p-4 space-y-3 sm:col-span-2">
          <h2 className="text-sm font-medium text-foreground/80">SLA</h2>
          <div className="flex items-center gap-2 text-sm">
            <Clock className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Time remaining</span>
            <span className="font-medium">{ticket.timeRemaining}</span>
          </div>
          {ticket.riskLevel !== "NoSla" && (
            <div className="flex items-center gap-2 text-sm">
              <Clock className="h-4 w-4 text-muted-foreground shrink-0" />
              <span className="text-muted-foreground">SLA deadline</span>
              <span className="font-medium">
                {formatDateTime(ticket.slaDeadlineAt)}
              </span>
            </div>
          )}
          <div className="flex items-center gap-2 text-sm">
            <Clock className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Created</span>
            <span className="font-medium">
              {formatDateTime(ticket.createdAt)}
            </span>
          </div>
          <div className="flex items-center gap-2 text-sm">
            <Clock className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-muted-foreground">Last updated</span>
            <span className="font-medium">
              {formatDateTime(ticket.updatedAt)}
            </span>
          </div>
        </div>
      </div>

      {/* Comments & activity */}
      <div className="rounded-lg border border-border/60 bg-card p-4">
        <Tabs defaultValue="comments">
          <TabsList>
            <TabsTrigger value="comments">Comments</TabsTrigger>
            <TabsTrigger value="activity">Activity</TabsTrigger>
          </TabsList>
          <TabsContent value="comments" className="pt-4">
            <TicketComments
              workspaceId={workspaceId!}
              ticketId={ticket.id}
              currentUserId={currentUser.id}
              isAdmin={currentUser.role === "Admin"}
              canComment={perms.canComment}
            />
          </TabsContent>
          <TabsContent value="activity" className="pt-4">
            <TicketActivityLog
              workspaceId={workspaceId!}
              ticketId={ticket.id}
            />
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
};

export default TicketDetailPage;
