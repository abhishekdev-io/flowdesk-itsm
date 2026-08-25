import { Shield, Headset, User, type LucideIcon } from "lucide-react";

const ROLE_STYLES: Record<string, { icon: LucideIcon; className: string }> = {
  Admin: {
    icon: Shield,
    className: "border-primary/40 bg-primary/10 text-primary",
  },
  Agent: {
    icon: Headset,
    className: "border-sky-500/40 bg-sky-500/10 text-sky-400",
  },
  Member: {
    icon: User,
    className: "border-emerald-500/40 bg-emerald-500/10 text-emerald-400",
  },
};

interface RoleBadgeProps {
  role: string;
}

const RoleBadge = ({ role }: RoleBadgeProps) => {
  const style = ROLE_STYLES[role] ?? ROLE_STYLES.Member;
  const Icon = style.icon;
  return (
    <div
      className={`flex items-center gap-1.5 rounded-full border px-2.5 py-1 font-mono text-[10px] uppercase tracking-wider ${style.className}`}
    >
      <Icon className="h-3 w-3" />
      {role}
    </div>
  );
};

export default RoleBadge;
 