import { useNavigate, useParams } from "react-router";
import { useState, useMemo } from "react";
import Spinner from "@/components/Spinner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type {
  TicketFilterParams,
  TicketSummaryResponse,
} from "@/types/ticket.types";
import { useTickets } from "@/hook/useTicketQueries";
import { useEntityAccess } from "@/hook/useEntityAccess";
import {
  Search,
  Plus,
  Filter,
  Ticket as TicketIcon,
  ChevronLeft,
  ChevronRight,
  AlertCircle,
  ArrowUpDown,
  CircleDot,
  Archive,
  UserX,
} from "lucide-react";
import { useDebounce } from "@/hook/useDebounce";
import CreateTicketDialog from "./components/CreateTicketDialog";
import { useMyWorkspaceRole } from "@/stores/useWorkspaceContext";
import { useAuth } from "@/stores/useAuth";

const PRIORITY_STYLES: Record<TicketSummaryResponse["priority"], string> = {
  Low: "text-muted-foreground bg-muted",
  Medium: "text-blue-700 bg-blue-50",
  High: "text-amber-700 bg-amber-50",
  Critical: "text-red-700 bg-red-50",
};

const RISK_STYLES: Record<TicketSummaryResponse["riskLevel"], string> = {
  NoSla: "text-muted-foreground bg-muted",
  OnTrack: "text-green-700 bg-green-50",
  AtRisk: "text-amber-700 bg-amber-50",
  Critical: "text-red-700 bg-red-50",
  Breached: "text-red-800 bg-red-100",
};

const SORT_MAP = {
  newest: { sortBy: "CreatedAt", descending: true },
  oldest: { sortBy: "CreatedAt", descending: false },
  priority: { sortBy: "Priority", descending: true },
} as const;

type SortKey = keyof typeof SORT_MAP;

const PAGE_SIZE = 20;

const TicketsPage = () => {
  const { workspaceId } = useParams();
  const navigate = useNavigate();
  const role = useMyWorkspaceRole();
  const auth = useAuth.getState().auth;
  const userId = auth.status === "authenticated" ? auth.user.userId : null;
  const canSeeMine = role === "Admin" || role === "Agent";
  const { canCreate } = useEntityAccess("tickets");
  const [createOpen, setCreateOpen] = useState(false);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const debouncedSearch = useDebounce(searchInput, 400);
  const [sortBy, setSortBy] = useState<SortKey>("newest");

  type AssignmentFilterValue = "all" | "unassigned" | "mine";

  const [assignmentFilter, setAssignmentFilter] =
    useState<AssignmentFilterValue>("all");

  const [archiveFilter, setArchiveFilter] = useState<"active" | "archived">(
    "active",
  );
  const [status, setStatus] = useState<TicketSummaryResponse["status"] | "all">(
    "all",
  );
  const [priority, setPriority] = useState<
    TicketSummaryResponse["priority"] | "all"
  >("all");

  const filters: TicketFilterParams = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      sortBy: SORT_MAP[sortBy].sortBy,
      descending: SORT_MAP[sortBy].descending,
      search: debouncedSearch || undefined,
      status: status === "all" ? undefined : status,
      priority: priority === "all" ? undefined : priority,
      isArchived: archiveFilter === "archived",
      isUnassigned: assignmentFilter === "unassigned" ? true : undefined,
      assignedToUserId:
        assignmentFilter === "mine" && canSeeMine
          ? (userId ?? undefined)
          : undefined,
    }),
    [
      page,
      sortBy,
      debouncedSearch,
      status,
      priority,
      archiveFilter,
      assignmentFilter,
      canSeeMine,
      userId,
    ],
  );

  const { data, isLoading, isFetching } = useTickets(workspaceId!, filters);
  const items = data?.items ?? [];

  const handleSearchChange = (value: string) => {
    setSearchInput(value);
    setPage(1);
  };

  const handleStatusChange = (
    value: TicketSummaryResponse["status"] | "all" | null,
  ) => {
    if (value === null) return;
    setStatus(value);
    setPage(1);
  };

  const handlePriorityChange = (
    value: TicketSummaryResponse["priority"] | "all" | null,
  ) => {
    if (value === null) return;
    setPriority(value);
    setPage(1);
  };

  const handleSortChange = (value: SortKey | null) => {
    if (value === null) return;
    setSortBy(value);
    setPage(1);
  };

  const handleArchiveChange = (value: "active" | "archived" | null) => {
    if (value === null) return;
    setArchiveFilter(value);
    setPage(1);
  };

  const handleAssignmentChange = (value: AssignmentFilterValue | null) => {
    if (value === null) return;
    setAssignmentFilter(value);
    setPage(1);
  };

  if (isLoading) return <Spinner />;

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Tickets</h1>
          <p className="text-sm text-muted-foreground">
            Manage workspace support requests, priorities, and SLA tracks.
          </p>
        </div>
        {canCreate && (
          <Button
            onClick={() => setCreateOpen(true)}
            className="gap-2 shrink-0"
          >
            <Plus className="h-4 w-4" />
            New ticket
          </Button>
        )}
      </div>

      {/* Toolbar */}
      <div className="flex flex-col sm:flex-row items-center gap-3 justify-between bg-card p-3 rounded-lg border border-border/60 shadow-xs">
        <div className="relative w-full sm:w-72">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search by ticket #, asset, or title..."
            value={searchInput}
            onChange={(e) => handleSearchChange(e.target.value)}
            className="pl-9 bg-background/50 text-sm"
          />
        </div>

        <div className="flex items-center gap-2 w-full sm:w-auto flex-wrap justify-end">
          <Filter className="h-4 w-4 text-muted-foreground shrink-0 hidden sm:block" />

          {/* Archive Filter Dropdown */}
          <Select value={archiveFilter} onValueChange={handleArchiveChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <Archive className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Archive:
                </span>
                <SelectValue placeholder="Active" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="active">Active Only</SelectItem>
              <SelectItem value="archived">Archived Only</SelectItem>
            </SelectContent>
          </Select>

          {/* Assignment Filter Dropdown */}
          <Select
            value={assignmentFilter}
            onValueChange={handleAssignmentChange}
          >
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <UserX className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Assignment:
                </span>
                <SelectValue placeholder="All" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All</SelectItem>
              <SelectItem value="unassigned">Unassigned Only</SelectItem>
              {canSeeMine && (
                <SelectItem value="mine">Assigned to Me</SelectItem>
              )}
            </SelectContent>
          </Select>

          {/*  Status Dropdown */}
          <Select value={status} onValueChange={handleStatusChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <CircleDot className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Status:
                </span>
                <SelectValue placeholder="All" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All</SelectItem>
              <SelectItem value="Open">Open</SelectItem>
              <SelectItem value="InProgress">In Progress</SelectItem>
              <SelectItem value="Resolved">Resolved</SelectItem>
              <SelectItem value="Closed">Closed</SelectItem>
            </SelectContent>
          </Select>

          {/*  Priority Dropdown */}
          <Select value={priority} onValueChange={handlePriorityChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <AlertCircle className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Priority:
                </span>
                <SelectValue placeholder="All" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All</SelectItem>
              <SelectItem value="Critical">Critical</SelectItem>
              <SelectItem value="High">High</SelectItem>
              <SelectItem value="Medium">Medium</SelectItem>
              <SelectItem value="Low">Low</SelectItem>
            </SelectContent>
          </Select>

          {/* Visual Separator between Filtering and Sorting */}
          <div className="h-4 w-px bg-border/60 mx-1 hidden sm:block" />

          {/*  Sort Dropdown */}
          <Select value={sortBy} onValueChange={handleSortChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <ArrowUpDown className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">Sort:</span>
                <SelectValue placeholder="Newest" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="newest">Newest First</SelectItem>
              <SelectItem value="oldest">Oldest First</SelectItem>
              <SelectItem value="priority">Priority</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Ticket List */}
      <div
        className={`space-y-2.5 transition-opacity ${isFetching ? "opacity-60" : "opacity-100"}`}
      >
        {items.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-center border border-dashed rounded-lg bg-card/40">
            <div className="flex h-10 w-10 items-center justify-center rounded-full bg-muted mb-3 text-muted-foreground">
              <TicketIcon className="h-5 w-5" />
            </div>
            <p className="text-sm font-medium">No tickets found</p>
            <p className="text-xs text-muted-foreground mt-1">
              {debouncedSearch
                ? `No ticket matching "${debouncedSearch}" was found.`
                : "No tickets in this workspace yet."}
            </p>
          </div>
        ) : (
          items.map((t) => (
            <div
              key={t.id}
              className="group p-4 rounded-lg border border-border/60 bg-card hover:bg-accent/20 transition-all flex flex-col md:flex-row md:items-center justify-between gap-4 shadow-2xs cursor-pointer"
              onClick={() =>
                navigate(`/workspaces/${workspaceId}/tickets/${t.id}`)
              }
            >
              <div className="space-y-1 min-w-0">
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="font-mono text-xs font-semibold text-primary">
                    {t.ticketNumber}
                  </span>
                  <span className="text-muted-foreground text-xs">•</span>
                  <h2 className="text-sm font-medium tracking-tight text-foreground truncate max-w-xl">
                    {t.title}
                  </h2>
                </div>
                <div className="flex items-center gap-2 text-xs text-muted-foreground flex-wrap">
                  {t.archivedAt && (
                    <>
                      <span className="rounded bg-muted px-1.5 py-0.5 text-[10px] font-semibold text-muted-foreground border border-border">
                        Archived
                      </span>
                      <span>•</span>
                    </>
                  )}
                  <span className="font-medium text-foreground/80">
                    {t.type}
                  </span>
                  <span>•</span>
                  <span>{t.status}</span>
                  <span>•</span>
                  <span>by {t.createdByUserName}</span>
                  {t.assignedToUserName ? (
                    <>
                      <span>•</span>
                      <span>assigned to {t.assignedToUserName}</span>
                    </>
                  ) : (
                    <>
                      <span>•</span>
                      <span className="text-amber-700 font-medium">
                        Unassigned
                      </span>
                    </>
                  )}
                  {t.assetName && (
                    <>
                      <span>•</span>
                      <span className="text-primary/90 font-medium">
                        {t.assetName}
                      </span>
                    </>
                  )}
                </div>
              </div>

              <div className="flex items-center gap-2 shrink-0 self-start md:self-center">
                <span
                  className={`text-xs px-2.5 py-1 rounded-md font-medium ${PRIORITY_STYLES[t.priority]}`}
                >
                  {t.priority}
                </span>
                {t.riskLevel !== "NoSla" && (
                  <span
                    className={`text-xs px-2.5 py-1 rounded-md font-medium ${RISK_STYLES[t.riskLevel]}`}
                  >
                    {t.riskLevel}
                  </span>
                )}
              </div>
            </div>
          ))
        )}
      </div>

      {/* Pagination */}
      {data && data.totalCount > 0 && (
        <div className="flex items-center justify-between pt-2">
          <p className="text-xs text-muted-foreground">
            Page {data.page} of {data.totalPages} • {data.totalCount} tickets
          </p>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasPreviousPage}
              onClick={() => setPage((p) => p - 1)}
              className="gap-1"
            >
              <ChevronLeft className="h-4 w-4" />
              Prev
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasNextPage}
              onClick={() => setPage((p) => p + 1)}
              className="gap-1"
            >
              Next
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}
      <CreateTicketDialog
        open={createOpen}
        onOpenChange={setCreateOpen}
        workspaceId={workspaceId!}
      />
    </div>
  );
};

export default TicketsPage;
