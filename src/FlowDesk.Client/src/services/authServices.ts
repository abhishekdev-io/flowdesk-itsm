import type {
  RegisterValues,
  LoginValues,
  AuthResponse,
} from "@/types/auth.types";
import apiClient from "../lib/apiClient";
import axios from "axios";
import type { User } from "@/types/user";
import extractErrorMessage from "@/lib/extractErrorMessage";

interface RefreshTokenResult {
  accessToken: string;
  user: {
    userId: string;
    username: string;
    email: string;
    avatarUrl: string;
  };
}

interface AuthResult {
  accessToken: string;
  user: {
    userId: string;
    username: string;
    email: string;
    avatarUrl: string;
  };
}

export const registerUser = async (
  values: RegisterValues,
): Promise<AuthResult> => {
  try {
    const response = await apiClient.post<AuthResponse>(
      "/auth/register",
      values,
    );
    const data = response.data;

    return {
      accessToken: data.accessToken,
      user: {
        userId: data.userId,
        username: data.userName,
        email: data.email,
        avatarUrl: data.avatarUrl,
      },
    };
  } catch (err) {
    throw new Error(extractErrorMessage(err), { cause: err });
  }
};

export const loginUser = async (values: LoginValues): Promise<AuthResult> => {
  try {
    const response = await apiClient.post<AuthResponse>("/auth/login", values);
    const data = response.data;

    return {
      accessToken: data.accessToken,
      user: {
        userId: data.userId,
        username: data.userName,
        email: data.email,
        avatarUrl: data.avatarUrl,
      },
    };
  } catch (err) {
    throw new Error(extractErrorMessage(err), { cause: err });
  }
};

export const getCurrentUserInfo = async (): Promise<User> => {
  try {
    const response = await apiClient.get<User>("/users/current");
    const data = response.data;
    console.log("Fetched user info:", data);

    return {
      userId: data.userId,
      username: data.username,
      email: data.email,
      avatarUrl: data.avatarUrl,
    };
  } catch (err) {
    throw new Error(extractErrorMessage(err), { cause: err });
  }
};

export const logoutUser = async (): Promise<void> => {
  try {
    await apiClient.post("/auth/logout");
  } catch (err) {
    console.warn("Server logout request failed:", err);
  }
};

const refreshClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7120/api",
  withCredentials: true,
  timeout: 10000,
});

export const refreshToken = async (): Promise<RefreshTokenResult> => {
  try {
    const response = await refreshClient.post<AuthResponse>(
      "/auth/refresh-token",
    );
    const { accessToken, userId, userName, email, avatarUrl } = response.data;

    return {
      accessToken,
      user: { userId, username: userName, email, avatarUrl },
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status !== 401) {
        console.error("Unexpected error during token refresh:", error);
      }
    }
    throw error;
  }
};
