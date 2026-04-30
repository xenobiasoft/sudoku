import { renderHook, waitFor } from '@testing-library/react';
import { act } from 'react';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { usePlayerService } from './usePlayerService';
import { apiClient } from '../api/apiClient';

const mockNavigate = vi.fn();
vi.mock('react-router-dom', () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock('../api/apiClient', () => ({
  apiClient: {
    createProfile: vi.fn(),
    getProfile: vi.fn(),
  },
}));

const mockLocalStorage = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
};

Object.defineProperty(window, 'localStorage', {
  value: mockLocalStorage,
  writable: true,
  configurable: true,
});

const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

function profileJson(profileId: string, alias: string): string {
  return JSON.stringify({ profileId, alias });
}

describe('usePlayerService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    consoleSpy.mockClear();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('initialization — no stored data', () => {
    it('should redirect to /create-profile when no profile or legacy alias exists', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);

      renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
      });
    });

    it('should not be initialized after redirecting', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
      });

      expect(result.current.isInitialized).toBe(false);
      expect(result.current.playerAlias).toBeNull();
      expect(result.current.profileId).toBeNull();
    });
  });

  describe('initialization — valid stored profile', () => {
    it('should use existing profile when backend returns 200', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('profile-123', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockResolvedValue({
        status: 200,
        data: { profileId: 'profile-123', alias: 'alice' },
      });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(result.current.playerAlias).toBe('alice');
      expect(result.current.profileId).toBe('profile-123');
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it('should re-create orphaned profile on 404 and use it', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('old-id', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 404, data: null });
      (apiClient.createProfile as ReturnType<typeof vi.fn>).mockResolvedValue({
        status: 201,
        data: { profileId: 'new-id', alias: 'alice' },
      });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(result.current.playerAlias).toBe('alice');
      expect(result.current.profileId).toBe('new-id');
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith(
        'sudoku-profile',
        expect.stringContaining('"alias":"alice"')
      );
    });

    it('should redirect when orphaned profile cannot be re-created', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('old-id', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 404, data: null });
      (apiClient.createProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 409, data: null });

      renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
      });
    });

    it('should set error state on transient backend failure (non-404) without redirecting', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('p1', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 500, data: null });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.isInitialized).toBe(false);
      expect(result.current.error).toMatch(/Backend unavailable/);
      expect(mockNavigate).not.toHaveBeenCalled();
    });
  });

  describe('initialization — legacy alias migration', () => {
    it('should migrate legacy sudoku-alias to new profile', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-alias') return 'OldAlias';
        return null;
      });
      (apiClient.createProfile as ReturnType<typeof vi.fn>).mockResolvedValue({
        status: 201,
        data: { profileId: 'migrated-id', alias: 'oldalias' },
      });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(apiClient.createProfile).toHaveBeenCalledWith('oldalias');
      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('sudoku-alias');
      expect(result.current.playerAlias).toBe('oldalias');
      expect(result.current.profileId).toBe('migrated-id');
    });

    it('should retry with a numeric suffix when legacy alias has a 409 conflict', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-alias') return 'bob';
        return null;
      });
      (apiClient.createProfile as ReturnType<typeof vi.fn>)
        .mockResolvedValueOnce({ status: 409, data: null })
        .mockResolvedValueOnce({ status: 201, data: { profileId: 'retry-id', alias: 'bob42' } });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(apiClient.createProfile).toHaveBeenCalledTimes(2);
      expect(result.current.playerAlias).toBe('bob42');
    });

    it('should redirect when legacy migration fails completely', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-alias') return 'failuser';
        return null;
      });
      (apiClient.createProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 500, data: null });

      renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
      });
    });

    it('should not retry when legacy alias is exactly 50 characters', async () => {
      const longAlias = 'a'.repeat(50);
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-alias') return longAlias;
        return null;
      });
      (apiClient.createProfile as ReturnType<typeof vi.fn>).mockResolvedValue({ status: 409, data: null });

      renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
      });

      expect(apiClient.createProfile).toHaveBeenCalledTimes(1);
    });
  });

  describe('error handling', () => {
    it('should set error state on network failure', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('p1', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockRejectedValue(new Error('Network failure'));

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.error).toBe('Network failure');
      expect(result.current.isInitialized).toBe(false);
    });

    it('should use fallback message for non-Error exceptions', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('p1', 'alice');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockRejectedValue('String error');

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.error).toBe('Failed to initialize player');
    });
  });

  describe('clearPlayer', () => {
    it('should clear player data and both localStorage keys', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('p1', 'testuser');
        return null;
      });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockResolvedValue({
        status: 200,
        data: { profileId: 'p1', alias: 'testuser' },
      });

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      act(() => {
        result.current.clearPlayer();
      });

      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('sudoku-profile');
      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('sudoku-alias');
      expect(result.current.playerAlias).toBeNull();
      expect(result.current.profileId).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.error).toBeNull();
    });
  });

  describe('concurrent initialization guard', () => {
    it('should prevent multiple simultaneous initialization calls', async () => {
      mockLocalStorage.getItem.mockImplementation((key: string) => {
        if (key === 'sudoku-profile') return profileJson('p1', 'alice');
        return null;
      });
      let resolveGetProfile: (value: unknown) => void;
      const getProfilePromise = new Promise(resolve => { resolveGetProfile = resolve; });
      (apiClient.getProfile as ReturnType<typeof vi.fn>).mockReturnValue(getProfilePromise);

      const { result } = renderHook(() => usePlayerService());

      // Trigger extra calls while first is pending
      act(() => {
        result.current.initializePlayer();
        result.current.initializePlayer();
      });

      act(() => {
        resolveGetProfile!({ status: 200, data: { profileId: 'p1', alias: 'alice' } });
      });

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(apiClient.getProfile).toHaveBeenCalledTimes(1);
    });
  });
});
