import { BrowserRouter, Route, Routes } from "react-router";
import { createRoot } from "react-dom/client";
import "./index.css";
import { App } from "./App";
import Login from "./pages/Login";
import Register from "./pages/Register";
import RootLayout from "./components/RootLayout";
import FlowDeskHub from "./pages/FlowdeskHub";
import ProtectedRoute from "./components/ProtectedRoute";
import WorkspaceDashboard from "./features/workspace/WorkspaceDashboard";
import TicketsPage from "./features/ticket/TicketsPage";
import { RoleGuard } from "./components/RoleGuard";
import AssetPage from "./features/asset/AssetPage";
import AssetCategoryPage from "./features/asset-category/AssetCategoryPage";
import NotificationPage from "./features/notification/NotificationPage";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "./lib/queryClient";
import WorkspaceHome from "./features/workspace/WorkspaceHome";
import SlaDashboardPage from "./features/ticket/SlaDashboardPage";
import MemberPage from "./features/workspace/MemberPage";
import CategorySettingsPage from "./components/admin-sidebar-settings/CategorySettingsPage";
import DangerZoneSettingsPage from "./components/admin-sidebar-settings/DangerZoneSettingsPage";
import TicketDetailPage from "./features/ticket/TicketDetailPage";
import WorkspaceGeneralSettingsPage from "./features/workspace/WorkspaceGeneralSettingsPage";
import ForbiddenPage from "./components/ForbiddenPage";

createRoot(document.getElementById("root")!).render(
  <QueryClientProvider client={queryClient}>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<RootLayout />}>
          <Route index element={<App />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/forbidden" element={<ForbiddenPage />} />
          <Route element={<ProtectedRoute />}>
            <Route path="/hub" element={<FlowDeskHub />} />
            <Route
              path="/workspaces/:workspaceId"
              element={<WorkspaceDashboard />}
            >
              <Route index element={<WorkspaceHome />} />
              <Route
                path="tickets"
                element={
                  <RoleGuard entity="tickets">
                    <TicketsPage />
                  </RoleGuard>
                }
              />
              <Route
                path="tickets/:ticketId"
                element={
                  <RoleGuard entity="tickets">
                    <TicketDetailPage />
                  </RoleGuard>
                }
              />
              <Route
                path="assets"
                element={
                  <RoleGuard entity="assets">
                    <AssetPage />
                  </RoleGuard>
                }
              />
              <Route
                path="assetcategories"
                element={
                  <RoleGuard entity="assetcategories">
                    <AssetCategoryPage />
                  </RoleGuard>
                }
              />
              <Route
                path="notifications"
                element={
                  <RoleGuard entity="notifications">
                    <NotificationPage />
                  </RoleGuard>
                }
              />
              <Route
                path="sladashboard"
                element={
                  <RoleGuard entity="sladashboard">
                    <SlaDashboardPage />
                  </RoleGuard>
                }
              />
              <Route
                path="members"
                element={
                  <RoleGuard entity="members">
                    <MemberPage />
                  </RoleGuard>
                }
              />
              <Route
                path="settings/categories"
                element={
                  <RoleGuard entity="settings">
                    <CategorySettingsPage />
                  </RoleGuard>
                }
              />
              <Route
                path="settings/danger"
                element={
                  <RoleGuard entity="settings">
                    <DangerZoneSettingsPage />
                  </RoleGuard>
                }
              />
              <Route
                path="settings/general"
                element={
                  <RoleGuard entity="settings">
                    <WorkspaceGeneralSettingsPage />
                  </RoleGuard>
                }
              />
            </Route>
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  </QueryClientProvider>,
);
