export const EM_DASH = '—';

// .NET serializes TimeSpan in the "c" format: [d.]hh:mm:ss[.fffffff] — so a duration
// past 24h carries a day prefix ("1.01:00:00") and sub-second precision carries a
// fractional tail ("00:00:01.5000000"). Both must parse, not just plain "HH:MM:SS".
const TIMESPAN_PATTERN = /^(?:(\d+)\.)?(\d{1,2}):(\d{2}):(\d{2})(?:\.\d+)?$/;

/**
 * Formats a backend TimeSpan for display as `[H:]MM:SS`. Hours are dropped when zero and
 * days fold into the hours segment. Sub-second precision is truncated.
 * A missing or unparseable duration — a difficulty with no wins, say — renders as `fallback`.
 */
export function formatDuration(playDuration: string | null | undefined, fallback: string = EM_DASH): string {
  if (!playDuration) return fallback;

  const match = TIMESPAN_PATTERN.exec(playDuration.trim());
  if (!match) return fallback;

  const [, days, hours, minutes, seconds] = match;
  const totalHours = Number(days ?? 0) * 24 + Number(hours);
  const pad = (v: string) => v.padStart(2, '0');

  return totalHours > 0
    ? `${totalHours}:${pad(minutes)}:${pad(seconds)}`
    : `${pad(minutes)}:${pad(seconds)}`;
}
