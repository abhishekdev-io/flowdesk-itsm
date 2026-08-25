import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { useUpdateTicketPriority, useUpdateTicketType } from "@/hook/useTicketQueries";
import type { TicketDetailResponse } from "@/types/ticket.types";
import { ChevronDown } from "lucide-react";


const PRIORITY_STYLES: Record<TicketDetailResponse["priority"], string> = {
  Low: "text-muted-foreground bg-muted",
  Medium: "text-blue-700 bg-blue-50 dark:text-blue-300 dark:bg-blue-950/40",
  High: "text-amber-700 bg-amber-50 dark:text-amber-300 dark:bg-amber-950/40",
  Critical: "text-red-700 bg-red-50 dark:text-red-300 dark:bg-red-950/40",
};
 
const PRIORITY_DOT: Record<TicketDetailResponse["priority"], string> = {
  Low: "bg-muted-foreground/50",
  Medium: "bg-blue-500",
  High: "bg-amber-500",
  Critical: "bg-red-500",
};
 
const PRIORITIES: TicketDetailResponse["priority"][] = [
  "Low",
  "Medium",
  "High",
  "Critical",
];
 
interface TicketPriorityMenuProps {
  workspaceId: string;
  ticket: TicketDetailResponse;
  editable: boolean;
}

export const TicketPriorityMenu = ({
  workspaceId,
  ticket,
  editable,
}: TicketPriorityMenuProps) => {
  const updatePriority = useUpdateTicketPriority(workspaceId, ticket.id);
 
  const badge = (
    <span
      className={`text-xs px-2.5 py-1 rounded-md font-medium inline-flex items-center gap-1 ${PRIORITY_STYLES[ticket.priority]}`}
    >
      {ticket.priority}
      {editable && <ChevronDown className="h-3 w-3 opacity-60" />}
    </span>
  );
 
  if (!editable) return badge;
 
  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        disabled={updatePriority.isPending}
        className="disabled:opacity-60 rounded-md"
      >
        {badge}
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {PRIORITIES.map((priority) => (
          <DropdownMenuItem
            key={priority}
            disabled={priority === ticket.priority}
            onClick={() => updatePriority.mutate({ priority })}
          >
            <span
              className={`h-2 w-2 rounded-full mr-2 ${PRIORITY_DOT[priority]}`}
            />
            {priority}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};



const TYPE_LABELS: Record<TicketDetailResponse["type"], string> = {
  Incident: "Incident",
  ServiceRequest: "Service request",
  Problem: "Problem",
  ChangeRequest: "Change request",
};
 
const TYPES: TicketDetailResponse["type"][] = [
  "Incident",
  "ServiceRequest",
  "Problem",
  "ChangeRequest",
];
 
interface TicketTypeMenuProps {
  workspaceId: string;
  ticket: TicketDetailResponse;
  editable: boolean;
}

export const TicketTypeMenu = ({
  workspaceId,
  ticket,
  editable,
}: TicketTypeMenuProps) => {
  const updateType = useUpdateTicketType(workspaceId, ticket.id);
 
  const label = (
    <span className="font-medium inline-flex items-center gap-1">
      {TYPE_LABELS[ticket.type]}
      {editable && <ChevronDown className="h-3 w-3 opacity-60" />}
    </span>
  );
 
  if (!editable) return label;
 
  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        disabled={updateType.isPending}
        className="disabled:opacity-60 rounded-md"
      >
        {label}
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {TYPES.map((type) => (
          <DropdownMenuItem
            key={type}
            disabled={type === ticket.type}
            onClick={() => updateType.mutate({ type: type })}
          >
            {TYPE_LABELS[type]}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};