import { useMemo } from "react";

const API_ORIGIN =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(
    /\/api\/?$/,
    "",
  ) ?? "";

export const resolveAvatarUrl = (
  avatarUrl?: string | null,
): string | undefined => {
  if (!avatarUrl || avatarUrl.trim() === "") return undefined;

  if (
    avatarUrl.startsWith("http://") ||
    avatarUrl.startsWith("https://") ||
    avatarUrl.startsWith("data:")
  ) {
    return avatarUrl;
  }

  const normalizedPath = avatarUrl.startsWith("/")
    ? avatarUrl
    : `/${avatarUrl}`;
  return `${API_ORIGIN}${normalizedPath}`;
};

export const useAvatarUrl = (avatarUrl?: string | null) => {
  return useMemo(() => resolveAvatarUrl(avatarUrl), [avatarUrl]);
};


export function getInitials(name: string): string {
  if (!name) return "?";
  return name
    .split(" ")
    .map((part) => part[0])
    .join("")
    .toUpperCase()
    .slice(0, 2);
}