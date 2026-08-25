import { useParams } from "react-router";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Users } from "lucide-react";
import Skeleton from "@/components/Skeleton";
import { useWorkspaceMembers } from "@/hook/useWorkspaceQueries";
import AddMemberDialog from "./components/AddMemberDialog";
import MemberRoleSelect from "./components/MemberRoleSelect";
import RemoveMemberDialog from "./components/RemoveMemberDialog";
import { resolveAvatarUrl } from "@/hook/useAvatarUrl";
import { useEntityAccess } from "@/hook/useEntityAccess";
import { Badge } from "@/components/ui/badge";

const MemberPage = () => {
  const { workspaceId = "" } = useParams();
  const { data: members, isLoading } = useWorkspaceMembers(workspaceId);
  const { canCreate, canUpdate } = useEntityAccess("members");
  const canManage = canCreate || canUpdate;
  
  return (
    <div className="flex h-full flex-col">
      {/* Header */}
      <div className="flex items-center justify-between border-b border-border/60 px-6 py-4">
        <div>
          <h1 className="text-lg font-semibold text-foreground">Members</h1>
          <p className="text-sm text-muted-foreground">
            Manage who has access to this workspace and their role.
          </p>
        </div>
        {canCreate && members && (
          <AddMemberDialog
            workspaceId={workspaceId}
            existingMembers={members}
          />
        )}
      </div>

      {/* Table */}
      <div className="flex-1 overflow-y-auto px-6 py-4">
        {isLoading ? (
          <div className="space-y-2">
            {Array.from({ length: 6 }).map((_, i) => (
              <Skeleton key={i} className="h-12 w-full" />
            ))}
          </div>
        ) : !members || members.length === 0 ? (
          <div className="flex flex-col items-center justify-center gap-2 py-16 text-center">
            <Users className="h-8 w-8 text-muted-foreground/50" />
            <p className="text-sm text-muted-foreground">
              No members yet. Add someone from the global user list.
            </p>
          </div>
        ) : (
          <div className="rounded-md border border-border">
            <Table>
              <TableHeader>
                <TableRow className="hover:bg-transparent">
                  <TableHead>Member</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>Role</TableHead>
                  {canManage && (
                    <TableHead className="w-12 text-right">Remove</TableHead>
                  )}
                </TableRow>
              </TableHeader>
              <TableBody>
                {members.map((member) => (
                  <TableRow key={member.userId}>
                    <TableCell>
                      <div className="flex items-center gap-2.5">
                        <Avatar className="h-7 w-7">
                          <AvatarImage
                            src={resolveAvatarUrl(member.avatarUrl)}
                            alt={member.username}
                          />
                          <AvatarFallback className="text-[10px]">
                            {member.username.slice(0, 2).toUpperCase()}
                          </AvatarFallback>
                        </Avatar>
                        <span className="text-sm font-medium">
                          {member.username}
                        </span>
                      </div>
                    </TableCell>
                    <TableCell className="text-sm text-muted-foreground">
                      {member.email}
                    </TableCell>
                    <TableCell>
                      {canUpdate ? (
                        <MemberRoleSelect
                          workspaceId={workspaceId}
                          userId={member.userId}
                          currentRole={member.role}
                        />
                      ) : (
                        <Badge variant="outline" className="font-normal capitalize">
                          {member.role.toLowerCase()}
                        </Badge>
                      )}
                    </TableCell>
                    {canManage && (
                      <TableCell className="text-right">
                        <RemoveMemberDialog
                          workspaceId={workspaceId}
                          userId={member.userId}
                          username={member.username}
                        />
                      </TableCell>
                    )}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </div>
    </div>
  );
};

export default MemberPage;
