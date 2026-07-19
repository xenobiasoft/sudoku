import { describe, it, expect } from 'vitest';
import { valueToSymbol, symbolToValue, valuesForSize } from './symbols';

describe('valueToSymbol', () => {
  it('renders single digits as-is', () => {
    expect(valueToSymbol(1)).toBe('1');
    expect(valueToSymbol(5)).toBe('5');
    expect(valueToSymbol(9)).toBe('9');
  });

  it('renders 10-16 as letters A-G', () => {
    expect(valueToSymbol(10)).toBe('A');
    expect(valueToSymbol(11)).toBe('B');
    expect(valueToSymbol(12)).toBe('C');
    expect(valueToSymbol(13)).toBe('D');
    expect(valueToSymbol(14)).toBe('E');
    expect(valueToSymbol(15)).toBe('F');
    expect(valueToSymbol(16)).toBe('G');
  });
});

describe('symbolToValue', () => {
  it('parses digits 1-9', () => {
    expect(symbolToValue('1')).toBe(1);
    expect(symbolToValue('5')).toBe(5);
    expect(symbolToValue('9')).toBe(9);
  });

  it('parses letters A-G, case-insensitively', () => {
    expect(symbolToValue('A')).toBe(10);
    expect(symbolToValue('a')).toBe(10);
    expect(symbolToValue('G')).toBe(16);
    expect(symbolToValue('g')).toBe(16);
    expect(symbolToValue('D')).toBe(13);
    expect(symbolToValue('d')).toBe(13);
  });

  it('returns null for out-of-range digits', () => {
    expect(symbolToValue('0')).toBeNull();
  });

  it('returns null for letters outside A-G', () => {
    expect(symbolToValue('h')).toBeNull();
    expect(symbolToValue('H')).toBeNull();
    expect(symbolToValue('z')).toBeNull();
  });

  it('returns null for empty string', () => {
    expect(symbolToValue('')).toBeNull();
  });

  it('returns null for multi-character input', () => {
    expect(symbolToValue('10')).toBeNull();
    expect(symbolToValue('AB')).toBeNull();
  });
});

describe('valuesForSize', () => {
  it('returns 1-9 for size 9', () => {
    expect(valuesForSize(9)).toEqual([1, 2, 3, 4, 5, 6, 7, 8, 9]);
  });

  it('returns 1-16 for size 16', () => {
    expect(valuesForSize(16)).toEqual(Array.from({ length: 16 }, (_, i) => i + 1));
  });
});
