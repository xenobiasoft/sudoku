import { describe, it, expect } from 'vitest';
import { formatDuration } from './timeUtils';

describe('formatDuration', () => {
  it('drops the hours segment when there are no hours', () => {
    expect(formatDuration('00:06:12')).toBe('06:12');
  });

  it('keeps the hours segment when there are hours', () => {
    expect(formatDuration('01:06:12')).toBe('1:06:12');
  });

  // .NET serializes TimeSpan in the "c" format, so these are shapes the backend really emits.
  it('truncates the fractional seconds .NET emits for sub-second precision', () => {
    expect(formatDuration('00:00:01.5000000')).toBe('00:01');
  });

  it('folds the day prefix .NET emits past 24h into the hours segment', () => {
    expect(formatDuration('1.01:00:00')).toBe('25:00:00');
  });

  it('handles a day prefix and fractional seconds together', () => {
    expect(formatDuration('2.03:04:05.1234567')).toBe('51:04:05');
  });

  it('returns an em dash for a null duration', () => {
    expect(formatDuration(null)).toBe('—');
  });

  it('returns the caller-supplied fallback when one is given', () => {
    expect(formatDuration(undefined, '00:00')).toBe('00:00');
  });

  it('returns the fallback for an unparseable duration', () => {
    expect(formatDuration('not-a-duration')).toBe('—');
  });
});
