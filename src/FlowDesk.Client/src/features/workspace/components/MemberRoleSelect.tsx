import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useChangeMemberRole } from "@/hook/useWorkspaceQueries";
import  { WorkspaceRole } from "@/types/workspace.types";

interface MemberRoleSelectProps {
  workspaceId: string;
  userId: string;
  currentRole: string;
  disabled?: boolean;
}

const MemberRoleSelect = ({
  workspaceId,
  userId,
  currentRole,
  disabled,
}: MemberRoleSelectProps) => {
  const { mutate, isPending } = useChangeMemberRole(workspaceId);

  return (
    <Select
      value={currentRole}
      disabled={disabled || isPending}
      onValueChange={(role) => {
        if (role === currentRole) return;
        mutate({ userId, payload: { role: (role ?? "") as WorkspaceRole }});
      }}
    >
      <SelectTrigger
        className={`h-8 w-36 border-border bg-background/60 text-xs ${
          isPending ? "opacity-60" : ""
        }`}
      >
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        {Object.values(WorkspaceRole).map((role) => (
          <SelectItem key={role} value={role} className="text-xs">
            {role}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
};

export default MemberRoleSelect;
