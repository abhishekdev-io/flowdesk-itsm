import { Outlet } from "react-router";
import Navbar from "./Navbar/Navbar";
import { useEffect } from "react";
import { useAuth } from "@/stores/useAuth";
import Spinner from "@/components/Spinner";
import { Toaster } from "sonner";

const RootLayout = () => {
  const hasBootstrapped = useAuth((s) => s.hasBootstrapped);
  const bootstrap = useAuth((s) => s.bootstrap);

  useEffect(() => {
    bootstrap();
  }, [bootstrap]);

  if (!hasBootstrapped) {
    return  <Spinner />;
  }

  return (
    <div className="flex h-dvh flex-col overflow-hidden">
      <Navbar />
      <main className="flex-1 min-h-0 overflow-y-auto">
        <Outlet />
      </main>
      <Toaster richColors position="top-right" />
    </div>
    
  );
};

export default RootLayout;
