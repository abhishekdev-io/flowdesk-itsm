import { useNavigate } from "react-router";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { LayoutDashboard, LogOut } from "lucide-react";
import { useAuth } from "@/stores/useAuth";
import { useAvatarUrl } from "@/hook/useAvatarUrl";

export const UserMenu = () => {
  const navigate = useNavigate();
  const auth = useAuth((state) => state.auth);

  const signout = useAuth((state) => state.signout);

  const user = auth.status === "authenticated" ? auth.user : null;

  const avatarSrc = useAvatarUrl(user?.avatarUrl);
  const initials = user?.username.slice(0, 2).toUpperCase();

  const handleLogout = async () => {
    await signout();
    navigate("/", { replace: true });
  };

  if (!user) return null;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger className="cursor-pointer flex items-center gap-2 rounded-md border border-border pl-1 pr-3 py-1 hover:border-accent transition-colors">
        {avatarSrc ? (
          <img
            src={avatarSrc}
            alt={user.username}
            className="h-6 w-6 rounded-full object-cover"
          />
        ) : (
          <span className="h-6 w-6 rounded-full bg-(--accent-soft) text-accent text-xs font-semibold flex items-center justify-center">
            {initials}
          </span>
        )}
        <span className="text-sm text-(--text) font-medium">
          {user.username}
        </span>
      </DropdownMenuTrigger>

      <DropdownMenuContent
        align="end"
        className="bg-(--bg) border-border text-(--text) w-56"
      >
        <div className="flex items-center gap-3 px-2 py-2">
          {avatarSrc ? (
            <img
              src={avatarSrc}
              alt={user.username}
              className="h-9 w-9 rounded-full object-cover"
            />
          ) : (
            <span className="h-9 w-9 rounded-full bg-(--accent-soft) text-accent text-sm font-semibold flex items-center justify-center">
              {initials}
            </span>
          )}
          <div className="flex flex-col overflow-hidden">
            <span className="text-sm font-medium truncate">
              {user.username}
            </span>
            <span className="text-xs text-(--text-muted) truncate">
              {user.email}
            </span>
          </div>
        </div>
        <DropdownMenuSeparator className="bg-border" />
        <DropdownMenuItem
          onClick={() => navigate("/hub")}
          className="gap-2 cursor-pointer"
        >
          <LayoutDashboard className="h-4 w-4 text-accent" />
          Hub
        </DropdownMenuItem>
        <DropdownMenuSeparator className="bg-border" />
        <DropdownMenuItem
          onClick={handleLogout}
          className="gap-2 cursor-pointer text-(--critical) focus:text-(--critical)"
        >
          <LogOut className="h-4 w-4" />
          Log out
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
