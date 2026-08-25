import { NavLink, useParams } from "react-router";
import { useMyWorkspaceRole } from "@/stores/useWorkspaceContext";
import { canAccess, type EntityKey } from "@/config/entityAccess";
import {
  Ticket,
  Boxes,
  Tags,
  Bell,
  LayoutDashboard,
  Users,
  ArrowUpRight,
} from "lucide-react";
import { useWorkspace } from "@/hook/useWorkspaceQueries";

interface QuickLink {
  label: string;
  description: string;
  path: string;
  icon: React.ComponentType<{ className?: string }>;
  entity: EntityKey;
}

const QUICK_LINKS: QuickLink[] = [
  {
    label: "SLA dashboard",
    description: "Track breach risk across open tickets",
    path: "sladashboard",
    icon: LayoutDashboard,
    entity: "sladashboard",
  },
  {
    label: "Tickets",
    description: "Triage, assign, and resolve requests",
    path: "tickets",
    icon: Ticket,
    entity: "tickets",
  },
  {
    label: "Assets",
    description: "See what's deployed and its status",
    path: "assets",
    icon: Boxes,
    entity: "assets",
  },
  {
    label: "Categories",
    description: "Manage how assets get classified",
    path: "assetcategories",
    icon: Tags,
    entity: "assetcategories",
  },
  {
    label: "Members",
    description: "Review who has access and their role",
    path: "members",
    icon: Users,
    entity: "members",
  },
  {
    label: "Notifications",
    description: "Catch up on what changed",
    path: "notifications",
    icon: Bell,
    entity: "notifications",
  },
];

const WorkspaceHome = () => {
  const { workspaceId } = useParams();
  const myRole = useMyWorkspaceRole();

  const { data: workspace, isLoading: workspaceLoading } = useWorkspace(
    workspaceId ?? "",
  );

  if (!workspaceId || !myRole) return null;

  const links = QUICK_LINKS.filter((item) =>
    canAccess(item.entity, "nav", myRole),
  );
  const hour = new Date().getHours();
  const greeting =
    hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";

  return (
    <div className="mx-auto max-w-5xl px-6 py-10">
      <div className="mb-10 space-y-2">
        <p className="font-mono text-[11px] uppercase tracking-widest text-muted-foreground/70">
          {greeting}
        </p>

        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">
            {workspaceLoading ? "Loading…" : (workspace?.name ?? workspaceId)}
          </h1>
          {workspace?.description && (
            <p className="mt-1 text-sm text-muted-foreground max-w-2xl whitespace-pre-line leading-relaxed">
              {workspace.description}
            </p>
          )}
        </div>

        <p className="pt-1 text-xs text-muted-foreground/80">
          Signed in as{" "}
          <span className="font-medium text-foreground">{myRole}</span> — here's
          what you can reach.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {links.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className="group relative flex flex-col gap-3 rounded-lg border border-border bg-background/40 p-4 transition-colors hover:border-primary/40 hover:bg-accent/20"
          >
            <div className="flex items-center justify-between">
              <div className="flex h-8 w-8 items-center justify-center rounded-md border border-border bg-accent/30 text-foreground">
                <item.icon className="h-4 w-4" />
              </div>
              <ArrowUpRight className="h-3.5 w-3.5 text-muted-foreground/50 opacity-0 transition-opacity group-hover:opacity-100" />
            </div>
            <div>
              <p className="text-sm font-medium text-foreground">
                {item.label}
              </p>
              <p className="mt-0.5 text-xs text-muted-foreground">
                {item.description}
              </p>
            </div>
          </NavLink>
        ))}
      </div>
    </div>
  );
};

export default WorkspaceHome;
