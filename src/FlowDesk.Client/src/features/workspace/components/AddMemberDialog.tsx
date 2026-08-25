import { useMemo, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { UserPlus, Check } from "lucide-react";
import type { User } from "@/types/user";
import { useGlobalUsers } from "@/hook/useGlobalUserQueries";
import { useAddMember } from "@/hook/useWorkspaceQueries";
import {
  WorkspaceRole,
  type WorkspaceUserResponse,
} from "@/types/workspace.types";
import { getInitials, resolveAvatarUrl } from "@/hook/useAvatarUrl";

interface AddMemberDialogProps {
  workspaceId: string;
  existingMembers: WorkspaceUserResponse[];
}

const AddMemberDialog = ({
  workspaceId,
  existingMembers,
}: AddMemberDialogProps) => {
  const [open, setOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [role, setRole] = useState<WorkspaceRole>(WorkspaceRole.Agent);

  const { data: globalUsers, isLoading } = useGlobalUsers(workspaceId);
  const { mutate: addMember, isPending } = useAddMember(workspaceId);

  const memberIds = useMemo(
    () => new Set(existingMembers.map((m) => m.userId)),
    [existingMembers],
  );

  const availableUsers = useMemo(
    () => (globalUsers ?? []).filter((u) => !memberIds.has(u.userId)),
    [globalUsers, memberIds],
  );

  const reset = () => {
    setSelectedUser(null);
    setRole(WorkspaceRole.Agent);
  };

  const handleSubmit = () => {
    if (!selectedUser) return;
    addMember(
      { userId: selectedUser.userId, role },
      {
        onSuccess: () => {
          setOpen(false);
          reset();
        },
      },
    );
  };

  return (
    <Dialog
      open={open}
      onOpenChange={(next) => {
        setOpen(next);
        if (!next) reset();
      }}
    >
      <DialogTrigger
        render={
          <Button size="sm" className="gap-1.5">
            <UserPlus className="h-4 w-4" />
            Add member
          </Button>
        }
      />
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Add member</DialogTitle>
          <DialogDescription>
            Search for a user and choose their role in this workspace.
          </DialogDescription>
        </DialogHeader>

        <Command className="rounded-md border border-border">
          <CommandInput placeholder="Search by name or email..." />
          <CommandList>
            {isLoading ? (
              <div className="p-4 text-center text-sm text-muted-foreground">
                Loading users...
              </div>
            ) : (
              <>
                <CommandEmpty>No matching users found.</CommandEmpty>
                <CommandGroup>
                  {availableUsers.map((user) => (
                    <CommandItem
                      key={user.userId}
                      value={`${user.username} ${user.email}`}
                      onSelect={() => setSelectedUser(user)}
                      className="gap-2"
                    > 
                      <Avatar className="h-6 w-6">
                        <AvatarImage src={resolveAvatarUrl(user.avatarUrl)} />
                        <AvatarFallback className="text-[10px]">
                          {getInitials(user.username)}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex flex-col">
                        <span className="text-sm">{user.username}</span>
                        <span className="text-xs text-muted-foreground">
                          {user.email}
                        </span>
                      </div>
                      {selectedUser?.userId === user.userId && (
                        <Check className="ml-auto h-4 w-4 text-primary" />
                      )}
                    </CommandItem>
                  ))}
                </CommandGroup>
              </>
            )}
          </CommandList>
        </Command>

        {selectedUser && (
          <div className="flex items-center justify-between rounded-md border border-border bg-accent/20 px-3 py-2">
            <span className="text-sm">
              Add <span className="font-medium">{selectedUser.username}</span>{" "}
              as
            </span>
            <Select
              value={role}
              onValueChange={(v) => setRole(v as WorkspaceRole)}
            >
              <SelectTrigger className="h-8 w-32.5 text-xs">
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
          </div>
        )}

        <DialogFooter>
          <Button variant="outline" onClick={() => setOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={!selectedUser || isPending}>
            {isPending ? "Adding..." : "Add member"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default AddMemberDialog;
