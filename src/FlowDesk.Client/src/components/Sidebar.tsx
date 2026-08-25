import { Link, NavLink, useLocation } from "react-router";
import { useMyWorkspaceRole } from "@/stores/useWorkspaceContext";
import { canAccess, type EntityKey } from "@/config/entityAccess";
import {
  Ticket,
  Boxes,
  Tags,
  Bell,
  LayoutDashboard,
  Users,
  ArrowLeft,
  ShieldAlert,
  Settings,
  Building2,
} from "lucide-react";
import Skeleton from "./Skeleton";
import { useEntityAccess } from "@/hook/useEntityAccess";

interface NavItem {
  label: string;
  path: string;
  icon: React.ComponentType<{ className?: string }>;
  entity: EntityKey;
}

interface NavGroup {
  label: string;
  items: NavItem[];
}

const NAV_GROUPS: NavGroup[] = [
  {
    label: "Overview",
    items: [
      {
        label: "SLA dashboard",
        path: "sladashboard",
        icon: LayoutDashboard,
        entity: "sladashboard",
      },
    ],
  },
  {
    label: "Operations",
    items: [
      { label: "Tickets", path: "tickets", icon: Ticket, entity: "tickets" },
      { label: "Assets", path: "assets", icon: Boxes, entity: "assets" },
    ],
  },
  {
    label: "Access",
    items: [
      { label: "Members", path: "members", icon: Users, entity: "members" },
    ],
  },
  {
    label: "Alerts",
    items: [
      {
        label: "Notifications",
        path: "notifications",
        icon: Bell,
        entity: "notifications",
      },
    ],
  },
];

const SETTINGS_NAV_ITEMS = [
  { label: "General", path: "settings/general", icon: Building2 },
  { label: "Asset categories", path: "settings/categories", icon: Tags },
  { label: "Danger zone", path: "settings/danger", icon: ShieldAlert },
];

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `group relative flex items-center gap-2.5 rounded-md py-2 pl-3 pr-2 text-sm transition-colors ${
    isActive
      ? "bg-accent/60 text-foreground"
      : "text-muted-foreground hover:bg-accent/30 hover:text-foreground"
  }`;

const activeBar = (isActive: boolean) =>
  `absolute left-0 top-1/2 h-4 w-0.5 -translate-y-1/2 rounded-full bg-primary transition-opacity ${
    isActive ? "opacity-100" : "opacity-0"
  }`;

const Sidebar = () => {
  const myRole = useMyWorkspaceRole();
  const location = useLocation();
  const { canView: canSeeSettings } = useEntityAccess("settings");

  // crude but effective: any path segment containing "/settings" flips the sidebar mode
  const inSettings = location.pathname.includes("/settings");

  if (!myRole) {
    return (
      <aside className="flex h-full w-60 shrink-0 flex-col gap-1 border-r border-border p-3">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} className="h-8 w-full" />
        ))}
      </aside>
    );
  }

  const groups = NAV_GROUPS.map((group) => ({
    ...group,
    items: group.items.filter((item) => canAccess(item.entity, "nav", myRole)),
  })).filter((group) => group.items.length > 0);

  return (
    <aside className="flex h-full w-60 shrink-0 flex-col border-r border-border bg-background/60">
      {/* Header — either "Back to Hub" or "Back to workspace" depending on mode */}
      <div className="border-b border-border/60 p-3">
        {inSettings ? (
          <NavLink
            to=""
            end
            className="flex items-center gap-2 rounded-md px-2.5 py-2 text-xs font-medium text-muted-foreground transition-colors hover:bg-accent/50 hover:text-foreground"
          >
            <ArrowLeft className="h-4 w-4 shrink-0" />
            <span>Back to workspace</span>
          </NavLink>
        ) : (
          <Link
            to="/hub"
            className="flex items-center gap-2 rounded-md px-2.5 py-2 text-xs font-medium text-muted-foreground transition-colors hover:bg-accent/50 hover:text-foreground"
          >
            <ArrowLeft className="h-4 w-4 shrink-0" />
            <span>Back to Hub</span>
          </Link>
        )}
      </div>

      {/* Nav — swaps between main nav and settings nav */}
      <nav className="flex-1 space-y-5 overflow-y-auto px-3 py-4">
        {inSettings ? (
          <div>
            <p className="mb-1.5 px-2 font-mono text-[10px] font-medium uppercase tracking-widest text-muted-foreground/70">
              Settings
            </p>
            <div className="space-y-0.5">
              {SETTINGS_NAV_ITEMS.map((item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={navLinkClass}
                >
                  {({ isActive }) => (
                    <>
                      <span className={activeBar(isActive)} />
                      <item.icon
                        className={`h-4 w-4 shrink-0 ${isActive ? "text-primary" : ""}`}
                      />
                      {item.label}
                    </>
                  )}
                </NavLink>
              ))}
            </div>
          </div>
        ) : (
          groups.map((group) => (
            <div key={group.label}>
              <p className="mb-1.5 px-2 font-mono text-[10px] font-medium uppercase tracking-widest text-muted-foreground/70">
                {group.label}
              </p>
              <div className="space-y-0.5">
                {group.items.map((item) => (
                  <NavLink
                    key={item.path}
                    to={item.path}
                    className={navLinkClass}
                  >
                    {({ isActive }) => (
                      <>
                        <span className={activeBar(isActive)} />
                        <item.icon
                          className={`h-4 w-4 shrink-0 ${isActive ? "text-primary" : ""}`}
                        />
                        {item.label}
                      </>
                    )}
                  </NavLink>
                ))}
              </div>
            </div>
          ))
        )}
      </nav>

      {/* Settings entry point (admin only) — hidden while already inside settings */}
      {canSeeSettings && !inSettings && (
        <div className="border-t border-border/60 px-3 py-2">
          <NavLink to="settings/general" className={navLinkClass}>
            {({ isActive }) => (
              <>
                <span className={activeBar(isActive)} />
                <Settings
                  className={`h-4 w-4 shrink-0 ${isActive ? "text-primary" : ""}`}
                />
                Settings
              </>
            )}
          </NavLink>
        </div>
      )}

      {/* Status ticker */}
      <div className="flex items-center gap-2 border-t border-border/60 px-4 py-3">
        <span className="h-1.5 w-1.5 rounded-full bg-emerald-500" />
        <span className="font-mono text-[10px] uppercase tracking-widest text-muted-foreground/70">
          All systems nominal
        </span>
      </div>
    </aside>
  );
};

export default Sidebar;
