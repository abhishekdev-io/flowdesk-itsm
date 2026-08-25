import { useParams } from "react-router";
import { useSlaDashboard } from "@/hook/useTicketQueries";
import Spinner from "@/components/Spinner";
import { Button } from "@/components/ui/button";
import {
  Gauge,
  AlertOctagon,
  AlertTriangle,
  Clock,
  CheckCircle2,
  CircleSlash,
  RefreshCw,
  TrendingUp,
} from "lucide-react";

type RiskKey = "breached" | "critical" | "atRisk" | "onTrack" | "noSla";

const RISK_CONFIG: Record<
  RiskKey,
  {
    label: string;
    icon: typeof AlertOctagon;
    color: string;
    bg: string;
    hex: string;
  }
> = {
  breached: {
    label: "Breached",
    icon: AlertOctagon,
    color: "text-red-700 dark:text-red-400",
    bg: "bg-red-500/10 border-red-500/20",
    hex: "#ef4444",
  },
  critical: {
    label: "Critical",
    icon: AlertTriangle,
    color: "text-red-600 dark:text-red-400",
    bg: "bg-red-500/5 border-red-500/15",
    hex: "#f97373",
  },
  atRisk: {
    label: "At Risk",
    icon: Clock,
    color: "text-amber-700 dark:text-amber-400",
    bg: "bg-amber-500/10 border-amber-500/20",
    hex: "#f59e0b",
  },
  onTrack: {
    label: "On Track",
    icon: CheckCircle2,
    color: "text-emerald-700 dark:text-emerald-400",
    bg: "bg-emerald-500/10 border-emerald-500/20",
    hex: "#10b981",
  },
  noSla: {
    label: "No SLA",
    icon: CircleSlash,
    color: "text-muted-foreground",
    bg: "bg-muted border-border",
    hex: "#94a3b8",
  },
};

const complianceTier = (pct: number) => {
  if (pct >= 95)
    return {
      label: "Healthy",
      hex: "#10b981",
      text: "text-emerald-600 dark:text-emerald-400",
    };
  if (pct >= 80)
    return {
      label: "Needs attention",
      hex: "#f59e0b",
      text: "text-amber-600 dark:text-amber-400",
    };
  return {
    label: "At risk",
    hex: "#ef4444",
    text: "text-red-600 dark:text-red-400",
  };
};

const SlaDashboardPage = () => {
  const { workspaceId } = useParams();
  const { data, isLoading, isFetching, refetch } = useSlaDashboard(
    workspaceId!,
  );

  if (isLoading) {
    return (
      <div className="flex h-[50vh] items-center justify-center">
        <Spinner />
      </div>
    );
  }

  if (!data) {
    return (
      <div className="p-6 max-w-5xl mx-auto">
        <div className="flex flex-col items-center justify-center py-16 text-center border border-dashed rounded-xl bg-card/30">
          <Gauge className="h-6 w-6 text-muted-foreground mb-3" />
          <p className="text-sm font-medium">SLA data unavailable</p>
          <p className="text-xs text-muted-foreground mt-1">
            Couldn't load compliance metrics for this workspace.
          </p>
          <Button
            variant="outline"
            size="sm"
            onClick={() => refetch()}
            className="mt-4 gap-1.5"
          >
            <RefreshCw className="h-3.5 w-3.5" />
            Retry
          </Button>
        </div>
      </div>
    );
  }

  const {
    total,
    breached,
    critical,
    atRisk,
    onTrack,
    noSla,
    compliancePercent,
  } = data;
  const tier = complianceTier(compliancePercent);

  const rows: { key: RiskKey; count: number }[] = [
    { key: "breached", count: breached },
    { key: "critical", count: critical },
    { key: "atRisk", count: atRisk },
    { key: "onTrack", count: onTrack },
    { key: "noSla", count: noSla },
  ];

  const pctOf = (n: number) => (total > 0 ? (n / total) * 100 : 0);

  return (
    <div className="p-6 max-w-5xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-foreground flex items-center gap-2">
            <Gauge className="h-5 w-5 text-primary" />
            SLA Dashboard
          </h1>
          <p className="text-sm text-muted-foreground mt-0.5">
            Workspace-wide service level compliance across active tickets.
          </p>
        </div>
        <Button
          variant="outline"
          size="sm"
          onClick={() => refetch()}
          disabled={isFetching}
          className="gap-1.5 shrink-0"
        >
          <RefreshCw
            className={`h-3.5 w-3.5 ${isFetching ? "animate-spin" : ""}`}
          />
          Refresh
        </Button>
      </div>

      {/* Compliance gauge + total */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="md:col-span-1 p-6 rounded-xl border border-border/70 bg-card flex flex-col items-center justify-center gap-3 shadow-2xs">
          <div
            className="relative h-36 w-36 rounded-full flex items-center justify-center"
            style={{
              background: `conic-gradient(${tier.hex} ${compliancePercent * 3.6}deg, rgba(148,163,184,0.15) 0deg)`,
            }}
          >
            <div className="h-28 w-28 rounded-full bg-card flex flex-col items-center justify-center">
              <span className="text-2xl font-bold tracking-tight text-foreground">
                {compliancePercent.toFixed(1)}%
              </span>
              <span className={`text-[11px] font-medium mt-0.5 ${tier.text}`}>
                {tier.label}
              </span>
            </div>
          </div>
          <p className="text-xs text-muted-foreground uppercase tracking-wider font-medium">
            SLA Compliance
          </p>
        </div>

        {/* Distribution bar + summary */}
        <div className="md:col-span-2 p-5 rounded-xl border border-border/70 bg-card shadow-2xs space-y-4">
          <div className="flex items-center justify-between">
            <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
              Risk Distribution
            </p>
            <p className="text-xs text-muted-foreground">
              <span className="font-semibold text-foreground">{total}</span>{" "}
              total tickets tracked
            </p>
          </div>

          {/* Stacked segmented bar */}
          <div className="h-3 w-full rounded-full overflow-hidden flex bg-muted">
            {rows.map(({ key, count }) =>
              count > 0 ? (
                <div
                  key={key}
                  style={{
                    width: `${pctOf(count)}%`,
                    backgroundColor: RISK_CONFIG[key].hex,
                  }}
                  title={`${RISK_CONFIG[key].label}: ${count}`}
                />
              ) : null,
            )}
          </div>

          {/* Legend */}
          <div className="grid grid-cols-2 sm:grid-cols-5 gap-2 pt-1">
            {rows.map(({ key }) => (
              <div key={key} className="flex items-center gap-1.5">
                <span
                  className="h-2 w-2 rounded-full shrink-0"
                  style={{ backgroundColor: RISK_CONFIG[key].hex }}
                />
                <span className="text-xs text-muted-foreground truncate">
                  {RISK_CONFIG[key].label}
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Stat cards */}
      <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
        {rows.map(({ key, count }) => {
          const config = RISK_CONFIG[key];
          const Icon = config.icon;
          return (
            <div
              key={key}
              className={`p-4 rounded-xl border shadow-2xs space-y-1 ${config.bg}`}
            >
              <div className="flex items-center justify-between text-muted-foreground">
                <span className="text-xs font-medium uppercase tracking-wider">
                  {config.label}
                </span>
                <Icon className={`h-4 w-4 ${config.color}`} />
              </div>
              <p
                className={`text-2xl font-bold tracking-tight ${config.color}`}
              >
                {count}
              </p>
              <p className="text-[11px] text-muted-foreground">
                {pctOf(count).toFixed(1)}% of total
              </p>
            </div>
          );
        })}
      </div>

      {/* Footer note / next-step hint */}
      {(breached > 0 || critical > 0) && (
        <div className="flex items-start gap-2.5 p-3.5 rounded-lg border border-red-500/20 bg-red-500/5">
          <TrendingUp className="h-4 w-4 text-red-600 dark:text-red-400 mt-0.5 shrink-0" />
          <p className="text-xs text-red-700 dark:text-red-400">
            {breached} ticket{breached !== 1 ? "s" : ""} breached and {critical}{" "}
            at critical risk. Review high-priority tickets to prevent further
            SLA impact.
          </p>
        </div>
      )}
    </div>
  );
};

export default SlaDashboardPage;
