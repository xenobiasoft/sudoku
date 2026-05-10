import { renderHook, act } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { usePlayerService } from './usePlayerService';

const store: Record<string, string> = {};

const mockLocalStorage = {
  getItem: vi.fn((key: string) => store[key] ?? null),
  setItem: vi.fn((key: string, value: string) => { store[key] = value; }),
  removeItem: vi.fn((key: string) => { delete store[key]; }),
};

Object.defineProperty(window, 'localStorage', {
  value: mockLocalStorage,
  writable: true,
  configurable: true,
});

const PROFILE_KEY = 'sudoku-profile';

describe('usePlayerService', () => {
  beforeEach(() => {
    Object.keys(store).forEach(k => delete store[k]);
    vi.clearAllMocks();
  });

  describe('returning player', () => {
    it('returns profile info when sudoku-profile exists', () => {
      store[PROFILE_KEY] = JSON.stringify({ profileId: 'p1', alias: 'alice' });
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.playerAlias).toBe('alice');
      expect(result.current.profileId).toBe('p1');
      expect(result.current.isInitialized).toBe(true);
      expect(result.current.isNewPlayer).toBe(false);
    });

    it('returns null for invalid stored profile', () => {
      store[PROFILE_KEY] = 'not-json';
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.isNewPlayer).toBe(true);
      expect(result.current.playerAlias).toBeNull();
    });

    it('returns null when stored profile is missing required fields', () => {
      store[PROFILE_KEY] = JSON.stringify({ profileId: '', alias: 'alice' });
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.isNewPlayer).toBe(true);
    });
  });

  describe('new player', () => {
    it('returns isNewPlayer=true when no storage data exists', () => {
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.playerAlias).toBeNull();
      expect(result.current.profileId).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.isNewPlayer).toBe(true);
    });
  });

  describe('clearPlayer', () => {
    it('clears profile and both localStorage keys', () => {
      store[PROFILE_KEY] = JSON.stringify({ profileId: 'p1', alias: 'alice' });
      const { result } = renderHook(() => usePlayerService());

      expect(result.current.isInitialized).toBe(true);

      act(() => { result.current.clearPlayer(); });

      expect(result.current.playerAlias).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.isNewPlayer).toBe(true);
      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith(PROFILE_KEY);
    });
  });

  describe('hook properties', () => {
    it('isLoading is always false', () => {
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.isLoading).toBe(false);
    });

    it('error is always null', () => {
      const { result } = renderHook(() => usePlayerService());
      expect(result.current.error).toBeNull();
    });

    it('initializePlayer is a no-op', async () => {
      const { result } = renderHook(() => usePlayerService());
      await expect(result.current.initializePlayer()).resolves.toBeUndefined();
    });
  });
});
