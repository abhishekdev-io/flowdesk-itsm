import React from "react";
import {
  Ticket,
  Clock,
  ShieldCheck,
  Bell,
  Boxes,
  History,
  ArrowRight,
  Circle,
} from "lucide-react";
import { NavLink } from "react-router";
import FlowDeskIcon from "@/components/FlowdeskIcon";

function GithubIcon({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="currentColor"
      className={className}
      aria-hidden="true"
    >
      <path d="M12 .5C5.73.5.5 5.73.5 12c0 5.08 3.29 9.39 7.86 10.91.57.1.78-.25.78-.55 0-.27-.01-1.17-.02-2.12-3.2.7-3.88-1.36-3.88-1.36-.52-1.33-1.28-1.69-1.28-1.69-1.04-.72.08-.7.08-.7 1.16.08 1.77 1.19 1.77 1.19 1.03 1.76 2.7 1.25 3.36.96.1-.75.4-1.25.73-1.54-2.56-.29-5.25-1.28-5.25-5.7 0-1.26.45-2.29 1.19-3.09-.12-.29-.52-1.47.11-3.06 0 0 .97-.31 3.18 1.18a11 11 0 0 1 5.79 0c2.2-1.49 3.17-1.18 3.17-1.18.64 1.59.24 2.77.12 3.06.74.8 1.19 1.83 1.19 3.09 0 4.43-2.7 5.41-5.27 5.69.42.36.78 1.07.78 2.15 0 1.55-.01 2.8-.01 3.18 0 .3.2.66.79.55A10.52 10.52 0 0 0 23.5 12C23.5 5.73 18.27.5 12 .5Z" />
    </svg>
  );
}

const tickets = [
  {
    id: "FD-1042",
    title: "Prod API returning 502",
    severity: "P1",
    sla: "00:14:22",
    slaTone: "critical",
    status: "Escalated",
    live: true,
  },
  {
    id: "FD-1038",
    title: "VPN access request",
    severity: "P3",
    sla: "On track",
    slaTone: "ok",
    status: "Assigned",
    live: false,
  },
  {
    id: "FD-1035",
    title: "Laptop replacement",
    severity: "P2",
    sla: "3h 40m left",
    slaTone: "warn",
    status: "In progress",
    live: false,
  },
  {
    id: "FD-1031",
    title: "Password reset",
    severity: "P4",
    sla: "Closed",
    slaTone: "muted",
    status: "Resolved",
    live: false,
  },
];

const features = [
  {
    icon: Ticket,
    title: "Ticket management",
    desc: "Full lifecycle from submission to closure, with a typed activity history for every action taken.",
  },
  {
    icon: Clock,
    title: "SLA enforcement",
    desc: "Breach detection and compliance rates computed against a testable time provider, not wall-clock guesses.",
  },
  {
    icon: ShieldCheck,
    title: "Workspace permissions",
    desc: "Role-based access scoped per workspace, checked on every request through the authorization pipeline.",
  },
  {
    icon: Bell,
    title: "Real-time notifications",
    desc: "Domain events fire in-app and channel notifications the moment a ticket changes state.",
  },
  {
    icon: Boxes,
    title: "Asset lifecycle",
    desc: "Track hardware and software through active, under-maintenance, and decommissioned states.",
  },
  {
    icon: History,
    title: "Full audit trail",
    desc: "Every status change, comment, and assignment recorded for compliance and accountability.",
  },
];

const lifecycle = [
  "Submitted",
  "Open",
  "Assigned",
  "In progress",
  "Resolved",
  "Closed",
];

const stack = [
  ".NET 10",
  "Clean Architecture",
  "DDD",
  "CQRS + MediatR",
  "PostgreSQL",
  "ASP.NET Identity",
  "React",
  "TypeScript",
];

const slaBySeverity = [
  { label: "P1 — Critical", value: 94 },
  { label: "P2 — High", value: 97 },
  { label: "P3 — Normal", value: 99.8 },
];

function toneClass(tone: string) {
  switch (tone) {
    case "critical":
      return "text-critical";
    case "warn":
      return "text-warn";
    case "ok":
      return "text-ok";
    default:
      return "text-muted";
  }
}

export default function Home() {
  return (
    <div className="fd-root min-h-screen">
      <style>{`
        /* ---------- Light theme (default) ---------- */
        .fd-root {
          --bg: #f6f7f9;
          --surface: #ffffff;
          --surface-2: #eef1f4;
          --border: #dfe4e9;
          --text: #101820;
          --text-muted: #5b6976;
          --accent: #0d8f7c;
          --accent-soft: rgba(13, 143, 124, 0.09);
          --accent-contrast: #f4fffb;
          --warn: #a2660a;
          --warn-soft: rgba(162, 102, 10, 0.1);
          --critical: #c23b30;
          --critical-soft: rgba(194, 59, 48, 0.1);
          --ok: #1c8a53;
          --shadow-sm: 0 1px 2px rgba(16, 24, 32, 0.04);
          --shadow-md: 0 8px 24px -12px rgba(16, 24, 32, 0.16);

          background: var(--bg);
          color: var(--text);
          font-family: 'Inter', ui-sans-serif, system-ui, sans-serif;
          transition: background 0.2s ease, color 0.2s ease;
        }

        /* ---------- Dark theme ---------- */
        .dark .fd-root {
          --bg: #0b131b;
          --surface: #111c26;
          --surface-2: #17232f;
          --border: #23323f;
          --text: #e7eef3;
          --text-muted: #8ea0ae;
          --accent: #4fd8c4;
          --accent-soft: rgba(79, 216, 196, 0.12);
          --accent-contrast: #06211c;
          --warn: #f2a93b;
          --warn-soft: rgba(242, 169, 59, 0.12);
          --critical: #f2665b;
          --critical-soft: rgba(242, 102, 91, 0.12);
          --ok: #62d693;
          --shadow-sm: none;
          --shadow-md: none;
        }

        .fd-root .font-display {
          font-family: 'Space Grotesk', ui-sans-serif, system-ui, sans-serif;
        }
        .fd-root .font-mono {
          font-family: 'JetBrains Mono', ui-monospace, SFMono-Regular, monospace;
        }
        .fd-root .card {
          background: var(--surface);
          border: 1px solid var(--border);
          border-radius: 0.75rem;
          box-shadow: var(--shadow-sm);
        }
        .fd-root .surface-2 { background: var(--surface-2); }
        .fd-root .border-hairline { border-color: var(--border); }
        .fd-root .text-muted { color: var(--text-muted); }
        .fd-root .text-accent { color: var(--accent); }
        .fd-root .text-critical { color: var(--critical); }
        .fd-root .text-warn { color: var(--warn); }
        .fd-root .text-ok { color: var(--ok); }
        .fd-root .bg-accent-soft { background: var(--accent-soft); }
        .fd-root .bg-critical-soft { background: var(--critical-soft); }

        .fd-root .btn-primary {
          background: var(--accent);
          color: var(--accent-contrast);
          font-weight: 600;
          box-shadow: var(--shadow-sm);
          transition: transform 0.15s ease, filter 0.15s ease, box-shadow 0.15s ease;
        }
        .fd-root .btn-primary:hover {
          filter: brightness(1.08);
          transform: translateY(-1px);
          box-shadow: var(--shadow-md);
        }
        .fd-root .btn-secondary {
          background: transparent;
          border: 1px solid var(--border);
          color: var(--text);
          transition: border-color 0.15s ease, background 0.15s ease;
        }
        .fd-root .btn-secondary:hover {
          border-color: var(--accent);
          background: var(--surface-2);
        }

        .fd-root .feature-card {
          transition: transform 0.15s ease, border-color 0.15s ease, box-shadow 0.15s ease;
        }
        .fd-root .feature-card:hover {
          transform: translateY(-2px);
          border-color: var(--accent);
          box-shadow: var(--shadow-md);
        }

        .fd-root .pulse-dot {
          position: relative;
        }
        .fd-root .pulse-dot::after {
          content: "";
          position: absolute;
          inset: 0;
          border-radius: 9999px;
          background: var(--critical);
          animation: fd-pulse 1.8s ease-out infinite;
        }
        @keyframes fd-pulse {
          0% { opacity: 0.55; transform: scale(1); }
          70% { opacity: 0; transform: scale(2.6); }
          100% { opacity: 0; transform: scale(2.6); }
        }
        @media (prefers-reduced-motion: reduce) {
          .fd-root .pulse-dot::after { animation: none; }
          .fd-root .feature-card:hover,
          .fd-root .btn-primary:hover { transform: none; }
        }

        .fd-root a:focus-visible,
        .fd-root button:focus-visible {
          outline: 2px solid var(--accent);
          outline-offset: 2px;
        }
      `}</style>

      {/* HERO */}
      <section className="mx-auto max-w-6xl px-6 py-20 grid lg:grid-cols-2 gap-12 items-center">
        <div>
          <span className="inline-block text-xs font-mono uppercase tracking-widest text-accent bg-accent-soft rounded-full px-3 py-1 mb-6">
            IT service management
          </span>
          <h1 className="font-display text-4xl sm:text-5xl leading-[1.08] tracking-tight mb-6">
            Every ticket, tracked from first report to final close.
          </h1>
          <p className="text-muted text-lg leading-relaxed mb-8 max-w-lg">
            FlowDesk is a full-stack ITSM platform for teams who need SLA
            enforcement, workspace-scoped permissions, and a real audit trail —
            not just a list of tickets.
          </p>
          <div className="flex flex-wrap items-center gap-3">
            <NavLink to={"/register"}>
              <button className="btn-primary rounded-md px-5 py-2.5 text-sm flex items-center gap-1.5">
                Get started <ArrowRight className="h-4 w-4" />
              </button>
            </NavLink>

            <button className="btn-secondary rounded-md px-5 py-2.5 text-sm flex items-center gap-2">
              <GithubIcon className="h-4 w-4" /> View source
            </button>
          </div>
        </div>

        {/* Signature element: live ticket board */}
        <div className="card overflow-hidden">
          <div className="flex items-center justify-between px-5 py-3 border-b border-hairline surface-2">
            <span className="font-mono text-xs uppercase tracking-widest text-muted">
              Active queue
            </span>
            <span className="flex items-center gap-2 text-xs font-mono text-muted">
              <span className="relative h-1.5 w-1.5 rounded-full bg-critical pulse-dot" />
              live
            </span>
          </div>
          <div className="px-5 py-2 grid grid-cols-[1fr_auto_auto_auto] gap-4 text-[11px] font-mono uppercase tracking-widest text-muted border-b border-hairline">
            <span className="py-2">Ticket</span>
            <span className="py-2">Severity</span>
            <span className="py-2">SLA</span>
            <span className="py-2">Status</span>
          </div>
          <div>
            {tickets.map((t) => (
              <div
                key={t.id}
                className="grid grid-cols-[1fr_auto_auto_auto] gap-4 items-center px-5 py-3.5 border-b border-hairline last:border-0"
              >
                <div className="min-w-0">
                  <div className="font-mono text-xs text-accent">{t.id}</div>
                  <div className="text-sm truncate">{t.title}</div>
                </div>
                <span className="font-mono text-xs text-muted">
                  {t.severity}
                </span>
                <span
                  className={`font-mono text-xs ${toneClass(t.slaTone)} flex items-center gap-1.5`}
                >
                  {t.live && <Circle className="h-2 w-2 fill-current" />}
                  {t.sla}
                </span>
                <span className="text-xs text-muted whitespace-nowrap">
                  {t.status}
                </span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* FEATURES */}
      <section id="features" className="mx-auto max-w-6xl px-6 py-24">
        <div className="max-w-xl mb-14">
          <h2 className="font-display text-3xl tracking-tight mb-4">
            Built for how service desks actually run.
          </h2>
          <p className="text-muted leading-relaxed">
            Every layer — domain, permissions, notifications — is designed
            around the same rule: nothing happens to a ticket that isn't
            recorded and enforced.
          </p>
        </div>
        <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-5">
          {features.map((f) => (
            <div key={f.title} className="feature-card card p-6">
              <div className="h-9 w-9 rounded-md bg-accent-soft flex items-center justify-center mb-4">
                <f.icon className="h-4.5 w-4.5 text-accent" />
              </div>
              <h3 className="font-display text-base mb-2">{f.title}</h3>
              <p className="text-sm text-muted leading-relaxed">{f.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* WORKFLOW */}
      <section id="workflow" className="border-t border-hairline surface-2">
        <div className="mx-auto max-w-6xl px-6 py-24">
          <div className="max-w-xl mb-14">
            <h2 className="font-display text-3xl tracking-tight mb-4">
              One lifecycle, no shortcuts.
            </h2>
            <p className="text-muted leading-relaxed">
              A ticket only moves forward through these six states — each
              transition writes to the activity log the domain layer reads from.
            </p>
          </div>
          <div className="flex flex-col md:flex-row md:items-center gap-6 md:gap-0">
            {lifecycle.map((step, i) => (
              <React.Fragment key={step}>
                <div className="flex md:flex-col items-center md:items-start gap-3 md:gap-3 md:flex-1">
                  <span className="font-mono text-xs text-accent">
                    {String(i + 1).padStart(2, "0")}
                  </span>
                  <span className="text-sm font-medium">{step}</span>
                </div>
                {i < lifecycle.length - 1 && (
                  <div className="hidden md:block h-px flex-1 bg-border mx-2 mt-5" />
                )}
              </React.Fragment>
            ))}
          </div>
        </div>
      </section>

      {/* SLA COMPLIANCE */}
      <section className="mx-auto max-w-6xl px-6 py-24 grid lg:grid-cols-[auto_1fr] gap-12 items-center">
        <div>
          <div className="font-display text-6xl tracking-tight text-accent mb-2">
            98.6%
          </div>
          <p className="text-muted text-sm max-w-[16rem]">
            Rolling SLA compliance rate, computed live from breach and
            time-remaining calculations.
          </p>
        </div>
        <div className="card p-6 space-y-5">
          {slaBySeverity.map((row) => (
            <div key={row.label}>
              <div className="flex items-center justify-between text-sm mb-1.5">
                <span className="text-muted">{row.label}</span>
                <span className="font-mono text-xs">{row.value}%</span>
              </div>
              <div className="h-1.5 rounded-full surface-2 overflow-hidden">
                <div
                  className="h-full rounded-full bg-accent"
                  style={{ width: `${row.value}%` }}
                />
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* CTA */}
      <section className="border-t border-hairline">
        <div className="mx-auto max-w-6xl px-6 py-20 text-center">
          <h2 className="font-display text-3xl tracking-tight mb-4">
            Spin up a workspace and watch a ticket move.
          </h2>
          <p className="text-muted mb-8 max-w-md mx-auto">
            Create an account, open a workspace, and file your first ticket in
            under a minute.
          </p>
          <NavLink to={"/register"}>
            <button className="btn-primary rounded-md px-6 py-3 text-sm inline-flex items-center gap-1.5">
              Get started <ArrowRight className="h-4 w-4" />
            </button>
          </NavLink>
        </div>
      </section>

      {/* FOOTER */}
      <footer className="border-t border-hairline surface-2">
        <div className="mx-auto max-w-6xl px-6 py-10 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-2 text-sm text-muted">
            <FlowDeskIcon className="h-4 w-4 rounded-[3px]" />
            <span>FlowDesk — ITSM, end to end.</span>
          </div>
          <div className="flex items-center gap-6 text-sm text-muted">
            <a href="#features" className="hover:text-accent transition-colors">
              Product
            </a>
            <a href="#workflow" className="hover:text-accent transition-colors">
              Workflow
            </a>
            <a
              href="#"
              className="hover:text-accent transition-colors flex items-center gap-1.5"
            >
              <GithubIcon className="h-3.5 w-3.5" /> GitHub
            </a>
          </div>
        </div>
        <div className="border-t border-hairline">
          <div className="mx-auto max-w-6xl px-6 py-3">
            <p className="font-mono text-[11px] text-muted tracking-wide opacity-70">
              Built with {stack.join(" · ")}
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}
