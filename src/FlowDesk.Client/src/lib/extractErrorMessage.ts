import axios from "axios";

interface ApiErrorResponse {
  error: string;
  message: string;
  details: Record<string, string[]> | null;
}

const extractErrorMessage = (err: unknown): string => {
  if (axios.isAxiosError<ApiErrorResponse>(err)) {
    if (!err.response) {
      return "Can't reach the server. Check your connection and try again.";
    }
    const { message, details } = err.response.data ?? {};
    const firstDetail = details ? Object.values(details)[0]?.[0] : undefined;
    return firstDetail ?? message ?? "Something went wrong. Please try again.";
  }
  return "Something went wrong. Please try again.";
};

export default extractErrorMessage;
