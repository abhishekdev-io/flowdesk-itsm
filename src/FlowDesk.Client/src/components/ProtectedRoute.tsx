import { useAuth } from "@/stores/useAuth";
import { Navigate, Outlet, useLocation } from "react-router";
import Spinner from "./Spinner";

export default function ProtectedRoute() {
  const status = useAuth((state) => state.auth.status);
  const location = useLocation();

  if (status === "loading" || status === "authenticating") {
    return  <Spinner />;
  }

  if (status !== "authenticated") {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return <Outlet />;
}

