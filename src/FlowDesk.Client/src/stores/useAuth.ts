import {
  loginUser,
  logoutUser,
  refreshToken,
  registerUser,
} from "@/services/authServices";
import type { LoginValues, RegisterValues } from "@/types/auth.types";
import type { User } from "@/types/user";
import { create } from "zustand";
import { persist } from "zustand/middleware";

const LOCAL_KEY = "app_state";

type AuthStatus =
  | { status: "loading"; user: null; error: null }
  | { status: "unauthenticated"; user: null; error: string | null }
  | { status: "authenticating"; user: null; error: null }
  | { status: "authenticated"; user: User; error: null };

type AuthState = {
  accessToken: string | null;
  auth: AuthStatus;
  signup: (registerData: RegisterValues) => Promise<void>;
  signin: (loginData: LoginValues) => Promise<void>;
  signout: () => Promise<void>;
  changeLocalAuthData: (accessToken: string | null, auth: AuthStatus) => void;
  hasBootstrapped: boolean;
  bootstrap: () => Promise<void>;
};

export const useAuth = create<AuthState>()(
  persist(
    (set, get) => ({
      accessToken: null,
      auth: { status: "loading", user: null, error: null },
      hasBootstrapped: false,

      bootstrap: async () => {
        if (get().auth.status !== "authenticated") {
          set({ hasBootstrapped: true });
          return;
        }
        try {
          const response = await refreshToken();
          set({
            accessToken: response.accessToken,
            auth: { status: "authenticated", user: response.user, error: null },
            hasBootstrapped: true,
          });
        } catch {
          set({
            accessToken: null,
            auth: { status: "unauthenticated", user: null, error: null },
            hasBootstrapped: true,
          });
        }
      },
      signup: async (registerData) => {
        set({ auth: { status: "authenticating", user: null, error: null } });
        try {
          const response = await registerUser(registerData);
          set({
            accessToken: response.accessToken,
            auth: { status: "authenticated", user: response.user, error: null },
          });
        } catch (error) {
          const message =
            error instanceof Error ? error.message : "Registration failed";
          set({
            accessToken: null,
            auth: { status: "unauthenticated", user: null, error: message },
          });
          return Promise.reject(error);
        }
      },
      signin: async (loginData) => {
        set({ auth: { status: "authenticating", user: null, error: null } });
        try {
          const response = await loginUser(loginData);
          set({
            accessToken: response.accessToken,
            auth: { status: "authenticated", user: response.user, error: null },
          });
        } catch (error) {
          const message =
            error instanceof Error ? error.message : "Login failed";
          set({
            accessToken: null,
            auth: { status: "unauthenticated", user: null, error: message },
          });
          return Promise.reject(error);
        }
      },
      signout: async () => {
        try {
          await logoutUser();
        } finally {
          set({
            accessToken: null,
            auth: { status: "unauthenticated", user: null, error: null },
          });
        }
      },
      changeLocalAuthData: (accessToken, auth) => {
        set({ accessToken, auth });
      },
    }),
    {
      name: LOCAL_KEY,
      version: 1,
      partialize: (state) => ({
        auth:
          state.auth.status === "authenticated"
            ? {
                status: "authenticated" as const,
                user: state.auth.user,
                error: null,
              }
            : { status: "unauthenticated" as const, user: null, error: null },
      }),
    },
  ),
);

export const useIsLoggedIn = () =>
  useAuth((state) => state.auth.status === "authenticated");
