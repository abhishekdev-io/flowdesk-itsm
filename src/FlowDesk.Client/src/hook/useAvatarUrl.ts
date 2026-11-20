import { useMemo } from "react";

const getApiOrigin = (): string => {
  const rawBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;
  if (!rawBaseUrl || rawBaseUrl.trim() === "") return "";

  const trimmed = rawBaseUrl.trim();
  try {
    if (trimmed.startsWith("http://") || trimmed.startsWith("https://")) {
      return new URL(trimmed).origin;
    }
  } catch {
    // Fallback to regex replacement if URL parsing fails
  }

  // Handle relative base paths like "/api/v1" or "/api"
  return trimmed
    .replace(/\/api(?:\/v\d+(?:\.\d+)?)?\/?$/i, "")
    .replace(/\/+$/, "");
};

const API_ORIGIN = getApiOrigin();

export const resolveAvatarUrl = (
  avatarUrl?: string | null,
): string | undefined => {
  if (!avatarUrl || typeof avatarUrl !== "string") return undefined;

  const trimmed = avatarUrl.trim();
  if (trimmed === "") return undefined;

  // External URLs, Data URIs, and local Blob previews
  if (
    trimmed.startsWith("http://") ||
    trimmed.startsWith("https://") ||
    trimmed.startsWith("data:") ||
    trimmed.startsWith("blob:")
  ) {
    return trimmed;
  }

  // Normalize Windows backslashes and ensure single leading slash
  const normalized = trimmed.replace(/\\/g, "/");
  const path = normalized.startsWith("/") ? normalized : `/${normalized}`;

  return API_ORIGIN ? `${API_ORIGIN}${path}` : path;
};

export const useAvatarUrl = (avatarUrl?: string | null) => {
  return useMemo(() => resolveAvatarUrl(avatarUrl), [avatarUrl]);
};

export function getInitials(name?: string | null): string {
  if (!name || name.trim() === "") return "?";
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 1) {
    return parts[0].slice(0, 2).toUpperCase();
  }
  return (parts[0][0] + (parts[1]?.[0] ?? "")).toUpperCase();
}
