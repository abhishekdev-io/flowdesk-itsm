import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { getInitials, resolveAvatarUrl } from "@/hook/useAvatarUrl";
import { useAssignTicketUser, useUnAssignTicketUser } from "@/hook/useTicketQueries";
import { useWorkspaceMembers } from "@/hook/useWorkspaceQueries";
import type { TicketDetailResponse } from "@/types/ticket.types";
import { ChevronDown, Loader2, UserMinus, UserX } from "lucide-react";
import { useMemo, useState } from "react";

interface TicketAssignMenuProps {
  workspaceId: string;
  ticket: TicketDetailResponse;
  canAssign: boolean;
  canUnassign: boolean;
}

const TicketAssignMenu = ({
  workspaceId,
  ticket,
  canAssign,
  canUnassign,
}: TicketAssignMenuProps) => {
  const [open, setOpen] = useState(false);
  const { data: members, isLoading: membersLoading } =
    useWorkspaceMembers(workspaceId);
  const { mutate: assignUser, isPending: isAssigning } = useAssignTicketUser(
    workspaceId,
    ticket.id,
  );
  const { mutate: unassignUser, isPending: isUnassigning } =
    useUnAssignTicketUser(workspaceId, ticket.id);

  const isPending = isAssigning || isUnassigning;

  const assignableAgents = useMemo(() => {
    if (!members) return [];
    return members.filter(
      (member) =>
        (member.role === "Agent") &&
        member.userId !== ticket.assignedToUser?.userId,
    );
  }, [members, ticket.assignedToUser?.userId]);

  if (!canAssign && !canUnassign) {
    return (
      <span className="font-medium">
        {ticket.assignedToUser?.username ?? "Unassigned"}
      </span>
    );
  }

  return (
    <div className="flex items-center gap-2">
      <DropdownMenu open={open} onOpenChange={setOpen}>
        <DropdownMenuTrigger
          disabled={isPending}
          render={
            <Button
              variant="outline"
              size="sm"
              className="h-7 text-xs font-normal gap-1.5 px-2"
            />
          }
        >
          {ticket.assignedToUser ? (
            <Avatar className="h-4 w-4 text-[9px]">
              <AvatarImage
                src={resolveAvatarUrl(ticket.assignedToUser?.avatarUrl)}
                alt={ticket.assignedToUser.username}
              />
              <AvatarFallback className="bg-primary/10 text-primary font-medium">
                {getInitials(ticket.assignedToUser.username)}
              </AvatarFallback>
            </Avatar>
          ) : null}
          <span className="font-medium text-foreground">
            {ticket.assignedToUser?.username ?? "Unassigned"}
          </span>
          <ChevronDown className="h-3 w-3 text-muted-foreground opacity-60" />
        </DropdownMenuTrigger>

        <DropdownMenuContent align="start" className="w-56">
          {canUnassign && ticket.assignedToUser && (
            <>
              <DropdownMenuItem
                className="text-destructive focus:text-destructive gap-2 text-xs cursor-pointer"
                onClick={() =>
                  unassignUser({ userId: ticket.assignedToUser!.userId })
                }
              >
                <UserMinus className="h-3.5 w-3.5" />
                <span>Unassign</span>
              </DropdownMenuItem>
              <DropdownMenuSeparator />
            </>
          )}

          {canAssign && (
            <>
              <DropdownMenuGroup>
                <DropdownMenuLabel className="text-[11px] font-semibold text-muted-foreground uppercase px-2 py-1 tracking-wider">
                  Assign to Agent
                </DropdownMenuLabel>

                <div className="max-h-56 overflow-y-auto">
                  {membersLoading ? (
                    <div className="flex items-center justify-center gap-2 py-4 text-xs text-muted-foreground">
                      <Loader2 className="h-3.5 w-3.5 animate-spin" />
                      <span>Loading agents...</span>
                    </div>
                  ) : assignableAgents.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-4 px-2 text-center text-xs text-muted-foreground gap-1">
                      <UserX className="h-4 w-4 text-muted-foreground/60" />
                      <span>No eligible agents</span>
                    </div>
                  ) : (
                    assignableAgents.map((agent) => (
                      <DropdownMenuItem
                        key={agent.userId}
                        className="flex items-center justify-between gap-2 text-xs py-1.5 cursor-pointer"
                        onClick={() => assignUser({ assigneeId: agent.userId })}
                      >
                        <div className="flex items-center gap-2 min-w-0">
                          <Avatar className="h-5 w-5 shrink-0 text-[10px]">
                            <AvatarImage
                              src={resolveAvatarUrl(agent.avatarUrl)}
                              alt={agent.username}
                            />
                            <AvatarFallback className="bg-primary/10 text-primary font-medium">
                              {getInitials(agent.username)}
                            </AvatarFallback>
                          </Avatar>
                          <span className="truncate font-medium">
                            {agent.username}
                          </span>
                        </div>

                        <span className="text-[10px] font-medium px-1.5 py-0.5 rounded bg-muted text-muted-foreground shrink-0 border border-border/50">
                          {agent.role}
                        </span>
                      </DropdownMenuItem>
                    ))
                  )}
                </div>
              </DropdownMenuGroup>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
};

export default TicketAssignMenu;
