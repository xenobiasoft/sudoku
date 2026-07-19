/**
 * The single place the 1-9 / A-G symbol mapping exists for the 16x16 board.
 * Wire format stays numeric everywhere; these helpers are presentation-only.
 */
export function valueToSymbol(value: number): string {
  return value >= 10 ? String.fromCharCode(55 + value) : String(value);
}

export function symbolToValue(ch: string): number | null {
  if (ch.length !== 1) return null;
  const upper = ch.toUpperCase();
  if (upper >= '1' && upper <= '9') return upper.charCodeAt(0) - '0'.charCodeAt(0);
  if (upper >= 'A' && upper <= 'G') return upper.charCodeAt(0) - 'A'.charCodeAt(0) + 10;
  return null;
}

export function valuesForSize(size: number): number[] {
  return Array.from({ length: size }, (_, i) => i + 1);
}
