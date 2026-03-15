import { renderHook, act, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { usePlayerService } from './usePlayerService';
import { apiClient } from '../api/apiClient';

vi.mock('../api/apiClient', () => ({
  apiClient: {
    createPlayer: vi.fn(),
    playerExists: vi.fn(),
  },
}));

const mockLocalStorage = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
};

Object.defineProperty(window, 'localStorage', {
  value: mockLocalStorage,
});

const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

describe('usePlayerService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    consoleSpy.mockClear();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('initialization', () => {
    it('should start with default state', () => {
      const { result } = renderHook(() => usePlayerService());

      expect(result.current.playerAlias).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.isLoading).toBe(true);
      expect(result.current.error).toBeNull();
    });

    it('should auto-initialize on mount when no stored alias exists', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.createPlayer as any).mockResolvedValue('new-player-123');

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(mockLocalStorage.getItem).toHaveBeenCalledWith('sudoku-alias');
      expect(apiClient.createPlayer).toHaveBeenCalled();
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('sudoku-alias', 'new-player-123');
      expect(result.current.playerAlias).toBe('new-player-123');
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it('should use existing valid player alias from localStorage', async () => {
      mockLocalStorage.getItem.mockReturnValue('existing-player-456');
      (apiClient.playerExists as any).mockResolvedValue(true);

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(mockLocalStorage.getItem).toHaveBeenCalledWith('sudoku-alias');
      expect(apiClient.playerExists).toHaveBeenCalledWith('existing-player-456');
      expect(apiClient.createPlayer).not.toHaveBeenCalled();
      expect(result.current.playerAlias).toBe('existing-player-456');
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it('should create new player when stored alias no longer exists on server', async () => {
      mockLocalStorage.getItem.mockReturnValue('invalid-player-789');
      (apiClient.playerExists as any).mockResolvedValue(false);
      (apiClient.createPlayer as any).mockResolvedValue('new-player-987');

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(apiClient.playerExists).toHaveBeenCalledWith('invalid-player-789');
      expect(apiClient.createPlayer).toHaveBeenCalled();
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('sudoku-alias', 'new-player-987');
      expect(result.current.playerAlias).toBe('new-player-987');
    });

    it('should create new player when playerExists API call fails', async () => {
      mockLocalStorage.getItem.mockReturnValue('existing-player-456');
      (apiClient.playerExists as any).mockRejectedValue(new Error('API Error'));
      (apiClient.createPlayer as any).mockResolvedValue('fallback-player-111');

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(apiClient.playerExists).toHaveBeenCalledWith('existing-player-456');
      expect(apiClient.createPlayer).toHaveBeenCalled();
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('sudoku-alias', 'fallback-player-111');
      expect(result.current.playerAlias).toBe('fallback-player-111');
    });
  });

  describe('error handling', () => {
    it('should handle createPlayer API failure gracefully', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.createPlayer as any).mockRejectedValue(new Error('Server Error'));

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.playerAlias).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.error).toBe('Server Error');
      expect(consoleSpy).toHaveBeenCalledWith('Failed to initialize player:', expect.any(Error));
    });

    it('should handle non-Error exceptions', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.createPlayer as any).mockRejectedValue('String error');

      const { result } = renderHook(() => usePlayerService());

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.error).toBe('Failed to initialize player');
    });
  });

  describe('manual initialization', () => {
    it('should allow manual re-initialization', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.createPlayer as any).mockResolvedValue('manual-player-222');

      const { result } = renderHook(() => usePlayerService());

      // Wait for initial auto-initialization to complete
      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      // Reset mocks and simulate manual re-initialization
      vi.clearAllMocks();
      mockLocalStorage.getItem.mockReturnValue('cached-player-333');
      (apiClient.playerExists as any).mockResolvedValue(true);

      await act(async () => {
        await result.current.initializePlayer();
      });

      expect(apiClient.playerExists).toHaveBeenCalledWith('cached-player-333');
      expect(result.current.playerAlias).toBe('cached-player-333');
    });

    it('should prevent multiple simultaneous initialization calls', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.createPlayer as any).mockImplementation(() => 
        new Promise(resolve => setTimeout(() => resolve('slow-player-444'), 100))
      );

      const { result } = renderHook(() => usePlayerService());

      // Make multiple rapid calls
      act(() => {
        result.current.initializePlayer();
        result.current.initializePlayer();
        result.current.initializePlayer();
      });

      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      // Should only have been called once (from auto-init)
      expect(apiClient.createPlayer).toHaveBeenCalledTimes(1);
    });
  });

  describe('clearPlayer', () => {
    it('should clear player data and localStorage', async () => {
      mockLocalStorage.getItem.mockReturnValue('player-to-clear-555');
      (apiClient.playerExists as any).mockResolvedValue(true);

      const { result } = renderHook(() => usePlayerService());

      // Wait for initialization
      await waitFor(() => {
        expect(result.current.isInitialized).toBe(true);
      });

      expect(result.current.playerAlias).toBe('player-to-clear-555');

      // Clear the player
      act(() => {
        result.current.clearPlayer();
      });

      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('sudoku-alias');
      expect(result.current.playerAlias).toBeNull();
      expect(result.current.isInitialized).toBe(false);
      expect(result.current.error).toBeNull();
    });
  });

  describe('loading states', () => {
    it('should manage loading state correctly during initialization', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      let resolveCreatePlayer: (value: string) => void;
      const createPlayerPromise = new Promise<string>(resolve => {
        resolveCreatePlayer = resolve;
      });
      (apiClient.createPlayer as any).mockReturnValue(createPlayerPromise);

      const { result } = renderHook(() => usePlayerService());

      // Should be loading initially
      expect(result.current.isLoading).toBe(true);
      expect(result.current.isInitialized).toBe(false);

      // Resolve the API call
      act(() => {
        resolveCreatePlayer!('async-player-666');
      });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.isInitialized).toBe(true);
      expect(result.current.playerAlias).toBe('async-player-666');
    });
  });
});