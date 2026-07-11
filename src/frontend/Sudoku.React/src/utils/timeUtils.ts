export const EM_DASH = '—';

/**
 * Formats a backend TimeSpan ("HH:MM:SS") for display. Hours are dropped when zero.
 * A missing duration — a difficulty with no wins, say — renders as `fallback`.
 */
export function formatDuration(playDuration: string | null | undefined, fallback: string = EM_DASH): string {
  if (!playDuration) return fallback;

  const [h = 0, m = 0, s = 0] = playDuration.split(':').map(Number);

  if ([h, m, s].some(Number.isNaN)) return fallback;

  const pad = (v: number) => v.toString().padStart(2, '0');

  return h > 0 ? `${h}:${pad(m)}:${pad(s)}` : `${pad(m)}:${pad(s)}`;
}
