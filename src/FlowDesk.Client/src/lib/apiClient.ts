import { useAuth } from "@/stores/useAuth";
import axios from "axios";
import { refreshToken } from "../services/authServices";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "https://localhost:7120/api",
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
  timeout: 10000,
});

apiClient.interceptors.request.use((config) => {
  const accessToken = useAuth.getState().accessToken;
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

let isRefreshing = false;

type PendingCallback = (token: string | null) => void;

let pending: PendingCallback[] = [];

function queueRequest(callback: PendingCallback) {
  pending.push(callback);
}

function resolvesQueue(newToken: string | null) {
  pending.forEach((callback) => callback(newToken));
  pending = [];
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const is401 = error.response?.status === 401;
    const original = error.config;
    if (!is401 || original._retry) {
      return Promise.reject(error);
    }

    original._retry = true;

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        queueRequest((newToken) => {
          if (!newToken) {
            reject(error);
            return;
          }
          original.headers.Authorization = `Bearer ${newToken}`;
          resolve(apiClient(original));
        });
      });
    }

    isRefreshing = true;
    try {
      const authResponse = await refreshToken();
      const newToken = authResponse?.accessToken;
      if (!newToken) {
        throw new Error("Failed to refresh token");
      }
      useAuth.getState().changeLocalAuthData(newToken, {
        status: "authenticated",
        user: authResponse.user,
        error: null,
      });
 
      resolvesQueue(newToken);
      original.headers.Authorization = `Bearer ${newToken}`;
      return apiClient(original);
    } catch (error) {
      resolvesQueue(null);
      useAuth.getState().signout();
      return Promise.reject(error);
    } finally {
      isRefreshing = false;
    }
  },
);

export default apiClient;
