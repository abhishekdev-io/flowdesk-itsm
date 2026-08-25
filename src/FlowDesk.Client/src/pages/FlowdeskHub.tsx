import { useState } from "react";
import { useNavigate } from "react-router";
import { Plus, Radio } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import CreateWorkspaceDialog from "@/features/workspace/components/CreateWorkspaceDialog";
import { useWorkspaces } from "@/hook/useWorkspaceQueries";

const ACCENTS = [
  {
    text: "text-emerald-400",
    bg: "bg-emerald-500/10",
    border: "border-emerald-500/20",
    dot: "bg-emerald-400",
  },
  {
    text: "text-cyan-400",
    bg: "bg-cyan-500/10",
    border: "border-cyan-500/20",
    dot: "bg-cyan-400",
  },
  {
    text: "text-violet-400",
    bg: "bg-violet-500/10",
    border: "border-violet-500/20",
    dot: "bg-violet-400",
  },
  {
    text: "text-amber-400",
    bg: "bg-amber-500/10",
    border: "border-amber-500/20",
    dot: "bg-amber-400",
  },
  {
    text: "text-rose-400",
    bg: "bg-rose-500/10",
    border: "border-rose-500/20",
    dot: "bg-rose-400",
  },
  {
    text: "text-sky-400",
    bg: "bg-sky-500/10",
    border: "border-sky-500/20",
    dot: "bg-sky-400",
  },
];

const getAccent = (id: string) => {
  const hash = [...id].reduce((acc, char) => acc + char.charCodeAt(0), 0);
  return ACCENTS[hash % ACCENTS.length];
};

const getInitials = (name: string) =>
  name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((word) => word[0]?.toUpperCase())
    .join("");

const formatRelativeTime = (isoDate: string) => {
  const diffMs = Date.now() - new Date(isoDate).getTime();
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
  if (diffDays <= 0) return "Created today";
  if (diffDays === 1) return "Created yesterday";
  if (diffDays < 30) return `Created ${diffDays}d ago`;
  const diffMonths = Math.floor(diffDays / 30);
  if (diffMonths < 12) return `Created ${diffMonths}mo ago`;
  return `Created ${Math.floor(diffMonths / 12)}y ago`;
};

const FlowDeskHub = () => {
  const [dialogOpen, setDialogOpen] = useState(false);
  const navigate = useNavigate();

  const { data: workspaces, isLoading, error } = useWorkspaces();
  
  

  return (
    <div className="min-h-full bg-background text-foreground">
      <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="mb-1 text-xs font-medium uppercase tracking-wider text-muted-foreground">
              Control room
            </p>
            <h1 className="text-2xl font-semibold tracking-tight">
              Workspaces
            </h1>
            <p className="mt-1 text-sm text-muted-foreground">
              Every board you're a part of, live status at a glance.
            </p>
          </div>
          <Button
            onClick={() => setDialogOpen(true)}
            className="w-full gap-2 sm:w-auto"
          >
            <Plus className="h-4 w-4" /> New workspace
          </Button>
        </div>
        {/* Error */}
        {error && (
          <div
            role="alert"
            className="mb-6 rounded-lg border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive"
          >
            {error instanceof Error ? error.message : "Couldn't load workspaces."}
          </div>
        )}
        {/* Loading */}
        {isLoading && (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 3 }).map((_, i) => (
              <Skeleton key={i} className="h-36 rounded-xl" />
            ))}
          </div>
        )}
        {/* Empty state */}
        {!isLoading && workspaces?.length === 0 && (
          <div className="rounded-xl border border-dashed border-border py-16 text-center">
            <Radio className="mx-auto mb-3 h-8 w-8 text-muted-foreground" />
            <p className="font-medium"> No workspaces yet </p>
            <p className="mt-1 mb-5 text-sm text-muted-foreground">
              Create one to start tracking tickets and assets.
            </p>
            <Button onClick={() => setDialogOpen(true)} variant="outline">
              Create your first workspace
            </Button>
          </div>
        )}
        {/* Workspace grid */}
        {!isLoading && workspaces && workspaces.length > 0 && (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {workspaces.map((ws) => {
              const accent = getAccent(ws.id);
              return (
                <button
                  key={ws.id}
                  onClick={() => navigate(`/workspaces/${ws.id}`)}
                  className="group relative rounded-xl border border-border bg-card p-5 text-left shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-md"
                >
                  <div className="flex items-start gap-3">
                    {/* Workspace icon */}
                    <span
                      className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-lg border font-mono text-sm font-semibold ${accent.bg} ${accent.border} ${accent.text}`}
                    >
                      {getInitials(ws.name)}
                    </span>
                    {/* Workspace information */}
                    <div className="min-w-0 flex-1">
                      <div className="flex items-center justify-between gap-2">
                        <h3 className="truncate text-base font-semibold">
                          {ws.name}
                        </h3>
                        <span
                          className={`h-2 w-2 shrink-0 rounded-full ${accent.dot} animate-pulse`}
                        />
                      </div>
                      <p className="mt-1 line-clamp-2 min-h-10 text-sm text-muted-foreground">
                        {ws.description || "No description yet."}
                      </p>
                    </div>
                  </div>
                  {/* Created time */}
                  <div className="mt-4 border-t border-border pt-3 text-xs font-mono text-muted-foreground">
                    {formatRelativeTime(ws.createdAt)}
                  </div>
                </button>
              );
            })}
            {/* New workspace card */}
            <button
              onClick={() => setDialogOpen(true)}
              className="flex min-h-40 flex-col items-center justify-center gap-2 rounded-xl border border-dashed border-border text-muted-foreground transition-colors hover:border-primary/40 hover:bg-accent hover:text-primary"
            >
              <Plus className="h-5 w-5" />
              <span className="text-sm font-medium"> New workspace </span>
            </button>
          </div>
        )}
      </div>
      <CreateWorkspaceDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
      />
    </div>
  );
};

export default FlowDeskHub;
