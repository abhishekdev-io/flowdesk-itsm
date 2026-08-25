import { useEffect, useState } from "react";
import { NavLink } from "react-router";
import { Button } from "../ui/button";
import { useIsLoggedIn } from "@/stores/useAuth";
import { UserMenu } from "./UserMenu";
import FlowDeskIcon from "../FlowdeskIcon";
import { Moon, Sun } from "lucide-react";

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  `text-sm transition-colors ${
    isActive ? "text-(--text)" : "text-(--text-muted) hover:text-(--text)"
  }`;

function ThemeToggle() {
  const [isDark, setIsDark] = useState<boolean>(() => {
    if (typeof window === "undefined") return false;
    const stored = localStorage.getItem("flowdesk-theme");
    if (stored) return stored === "dark";
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
  });

  useEffect(() => {
    document.documentElement.classList.toggle("dark", isDark);
    localStorage.setItem("flowdesk-theme", isDark ? "dark" : "light");
  }, [isDark]);

  return (
    <button
      type="button"
      onClick={() => setIsDark((d) => !d)}
      aria-label={isDark ? "Switch to light theme" : "Switch to dark theme"}
      className="cursor-pointer h-8 w-8 rounded-md flex items-center justify-center border border-border text-(--text-muted) hover:text-(--text) hover:border-accent transition-colors"
    >
      {isDark ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
    </button>
  );
}

const Navbar = () => {
  const isLoggedIn = useIsLoggedIn();

  return (
    <nav className="sticky top-0 z-50 flex flex-col md:flex-row gap-4 md:gap-0 md:h-16 py-5 md:py-0 justify-around items-center bg-(--bg)/90 backdrop-blur border-b border-border">
      {/* brand */}
      <NavLink to="/" className="flex items-center gap-2.5 font-semibold group">
        <FlowDeskIcon className="h-7 w-7 rounded-md transition-transform group-hover:scale-105" />
        <span className="font-display tracking-tight text-xl text-(--text)">
          FlowDesk
        </span>
      </NavLink>

      <div className="flex gap-4 md:gap-6 items-center">
        <NavLink to="/" className={navLinkClass} end>
          Home
        </NavLink>

        <ThemeToggle />

        {isLoggedIn ? (
          <UserMenu />
        ) : (
          <>
            <NavLink to="/login">
              <Button
                variant="outline"
                className="cursor-pointer bg-transparent border-border text-(--text) hover:bg-(--surface-2) hover:border-accent hover:text-(--text)"
              >
                Sign in
              </Button>
            </NavLink>

            <NavLink to="/register">
              <Button className="cursor-pointer bg-accent text-(--accent-contrast) font-semibold hover:brightness-110">
                Sign up
              </Button>
            </NavLink>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
