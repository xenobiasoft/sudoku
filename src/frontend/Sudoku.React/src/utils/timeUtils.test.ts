import { describe, it, expect } from 'vitest';
import { formatDuration } from './timeUtils';

describe('formatDuration', () => {
  it('drops the hours segment when there are no hours', () => {
    expect(formatDuration('00:06:12')).toBe('06:12');
  });

  it('keeps the hours segment when there are hours', () => {
    expect(formatDuration('01:06:12')).toBe('1:06:12');
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
