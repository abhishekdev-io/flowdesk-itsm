export const formatDateTime = (value?: string | null) => {
  if (!value) return "—";
  const date = new Date(value);
  if (isNaN(date.getTime())) return "—";
  return date.toLocaleString();
};
 
const DIVISIONS: [Intl.RelativeTimeFormatUnit, number][] = [
  ["year", 60 * 60 * 24 * 365],
  ["month", 60 * 60 * 24 * 30],
  ["week", 60 * 60 * 24 * 7],
  ["day", 60 * 60 * 24],
  ["hour", 60 * 60],
  ["minute", 60],
  ["second", 1],
];
 
export const formatRelativeTime = (value?: string | null) => {
  if (!value) return "—";
  const date = new Date(value);
  if (isNaN(date.getTime())) return "—";
 
  const diffSec = Math.round((date.getTime() - Date.now()) / 1000);
  const rtf = new Intl.RelativeTimeFormat(undefined, { numeric: "auto" });
 
  for (const [unit, secondsInUnit] of DIVISIONS) {
    if (Math.abs(diffSec) >= secondsInUnit || unit === "second") {
      return rtf.format(Math.round(diffSec / secondsInUnit), unit);
    }
  }
  return "just now";
};