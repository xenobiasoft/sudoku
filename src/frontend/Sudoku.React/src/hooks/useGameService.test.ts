import { renderHook, act, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { useGameService } from './useGameService';
import { apiClient } from '../api/apiClient';
import type { GameModel } from '../types';

// Mock the apiClient
vi.mock('../api/apiClient', () => ({
  apiClient: {
    getGames: vi.fn(),
    deleteGame: vi.fn(),
  },
}));

// Mock localStorage
const mockLocalStorage = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
};

Object.defineProperty(window, 'localStorage', {
  value: mockLocalStorage,
});

// Mock console.error to avoid noise in tests
const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

// Sample game data for testing
const mockGames: GameModel[] = [
  {
    id: 'game-1',
    playerAlias: 'test-player',
    status: 'In Progress',
    difficulty: 'Easy',
    createdAt: '2024-01-01T00:00:00Z',
    startedAt: '2024-01-01T00:00:00Z',
    completedAt: null,
    pausedAt: null,
    cells: [],
    moveHistory: [],
    statistics: {
      totalMoves: 5,
      invalidMoves: 0,
      playDuration: '00:05:00',
    },
  },
  {
    id: 'game-2',
    playerAlias: 'test-player',
    status: 'Completed',
    difficulty: 'Medium',
    createdAt: '2024-01-02T00:00:00Z',
    startedAt: '2024-01-02T00:00:00Z',
    completedAt: '2024-01-02T02:00:00Z',
    pausedAt: null,
    cells: [],
    moveHistory: [],
    statistics: {
      totalMoves: 15,
      invalidMoves: 2,
      playDuration: '00:15:00',
    },
  },
  {
    id: 'game-3',
    playerAlias: 'test-player',
    status: 'In Progress',
    difficulty: 'Hard',
    createdAt: '2024-01-03T00:00:00Z',
    startedAt: '2024-01-03T00:00:00Z',
    completedAt: null,
    pausedAt: null,
    cells: [],
    moveHistory: [],
    statistics: {
      totalMoves: 25,
      invalidMoves: 5,
      playDuration: '00:20:00',
    },
  },
];

describe('useGameService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    consoleSpy.mockClear();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('initialization', () => {
    it('should start with default state', () => {
      const { result } = renderHook(() => useGameService());

      expect(result.current.savedGames).toEqual([]);
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
      expect(result.current.isLoaded).toBe(false);
    });
  });

  describe('loadGames', () => {
    it('should load games from localStorage cache first', async () => {
      const cachedGames = JSON.stringify(mockGames);
      mockLocalStorage.getItem.mockReturnValue(cachedGames);

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('test-player');
      });

      expect(mockLocalStorage.getItem).toHaveBeenCalledWith('savedGames');
      expect(apiClient.getGames).not.toHaveBeenCalled();
      expect(result.current.savedGames).toEqual([mockGames[0], mockGames[2]]); // Only non-completed games
      expect(result.current.isLoaded).toBe(true);
      expect(result.current.isLoading).toBe(false);
    });

    it('should fallback to API when no cached games exist', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.getGames as any).mockResolvedValue(mockGames);

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('test-player');
      });

      expect(mockLocalStorage.getItem).toHaveBeenCalledWith('savedGames');
      expect(apiClient.getGames).toHaveBeenCalledWith('test-player');
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('savedGames', JSON.stringify(mockGames));
      expect(result.current.savedGames).toEqual([mockGames[0], mockGames[2]]);
      expect(result.current.isLoaded).toBe(true);
    });

    it('should fallback to API when cached data is corrupted', async () => {
      mockLocalStorage.getItem.mockReturnValue('invalid-json');
      (apiClient.getGames as any).mockResolvedValue(mockGames);

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('test-player');
      });

      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('savedGames');
      expect(apiClient.getGames).toHaveBeenCalledWith('test-player');
      expect(result.current.savedGames).toEqual([mockGames[0], mockGames[2]]);
    });

    it('should force refresh from API when forceRefresh is true', async () => {
      const cachedGames = JSON.stringify(mockGames);
      mockLocalStorage.getItem.mockReturnValue(cachedGames);
      (apiClient.getGames as any).mockResolvedValue([mockGames[0]]);

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('test-player', true);
      });

      expect(apiClient.getGames).toHaveBeenCalledWith('test-player');
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('savedGames', JSON.stringify([mockGames[0]]));
      expect(result.current.savedGames).toEqual([mockGames[0]]);
    });

    it('should handle API errors gracefully', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      (apiClient.getGames as any).mockRejectedValue(new Error('API Error'));

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('test-player');
      });

      expect(result.current.savedGames).toEqual([]);
      expect(result.current.error).toBe('API Error');
      expect(result.current.isLoaded).toBe(true);
      expect(result.current.isLoading).toBe(false);
      expect(consoleSpy).toHaveBeenCalledWith('Failed to load games:', expect.any(Error));
    });

    it('should not load if player alias is empty', async () => {
      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.loadGames('');
      });

      expect(mockLocalStorage.getItem).not.toHaveBeenCalled();
      expect(apiClient.getGames).not.toHaveBeenCalled();
      expect(result.current.isLoaded).toBe(false);
    });

    it('should prevent multiple simultaneous loads', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      let callCount = 0;
      (apiClient.getGames as any).mockImplementation(() => {
        callCount++;
        return new Promise(resolve => setTimeout(() => resolve(mockGames), 100));
      });

      const { result } = renderHook(() => useGameService());

      // Start multiple loads simultaneously
      act(() => {
        result.current.loadGames('test-player');
        result.current.loadGames('test-player');
        result.current.loadGames('test-player');
      });

      await waitFor(() => {
        expect(result.current.isLoaded).toBe(true);
      });

      // API should only be called once due to the isLoading guard
      expect(callCount).toBe(1);
    });
  });

  describe('deleteGame', () => {
    beforeEach(() => {
      // Setup initial state with cached games
      const cachedGames = JSON.stringify(mockGames);
      mockLocalStorage.getItem.mockReturnValue(cachedGames);
    });

    it('should delete game from API and update local state', async () => {
      (apiClient.deleteGame as any).mockResolvedValue(undefined);

      const { result } = renderHook(() => useGameService());

      // Load initial games
      await act(async () => {
        await result.current.loadGames('test-player');
      });

      // Delete a game
      await act(async () => {
        await result.current.deleteGame('test-player', 'game-1');
      });

      expect(apiClient.deleteGame).toHaveBeenCalledWith('test-player', 'game-1');
      expect(result.current.savedGames).toEqual([mockGames[2]]); // Only game-3 should remain
      expect(mockLocalStorage.setItem).toHaveBeenCalledWith('savedGames', 
        JSON.stringify(mockGames.filter(g => g.id !== 'game-1'))
      );
    });

    it('should handle API delete errors', async () => {
      (apiClient.deleteGame as any).mockRejectedValue(new Error('Delete failed'));

      const { result } = renderHook(() => useGameService());

      // Load initial games
      await act(async () => {
        await result.current.loadGames('test-player');
      });

      // Attempt to delete a game
      await act(async () => {
        try {
          await result.current.deleteGame('test-player', 'game-1');
        } catch {
          // Expected to throw
        }
      });

      expect(result.current.error).toBe('Delete failed');
      expect(result.current.savedGames).toEqual([mockGames[0], mockGames[2]]); // State should be unchanged
      expect(consoleSpy).toHaveBeenCalledWith('Failed to delete game:', expect.any(Error));
    });

    it('should handle corrupted localStorage during delete', async () => {
      (apiClient.deleteGame as any).mockResolvedValue(undefined);
      mockLocalStorage.getItem.mockReturnValue('invalid-json');

      const { result } = renderHook(() => useGameService());

      // Load initial games from API
      (apiClient.getGames as any).mockResolvedValue(mockGames);
      await act(async () => {
        await result.current.loadGames('test-player');
      });

      // Delete a game
      await act(async () => {
        await result.current.deleteGame('test-player', 'game-1');
      });

      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('savedGames');
      expect(result.current.savedGames).toEqual([mockGames[2]]);
    });

    it('should not delete if player alias is empty', async () => {
      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.deleteGame('', 'game-1');
      });

      expect(apiClient.deleteGame).not.toHaveBeenCalled();
    });
  });

  describe('refreshGames', () => {
    it('should force reload games from API', async () => {
      const cachedGames = JSON.stringify(mockGames);
      mockLocalStorage.getItem.mockReturnValue(cachedGames);
      (apiClient.getGames as any).mockResolvedValue([mockGames[0]]);

      const { result } = renderHook(() => useGameService());

      await act(async () => {
        await result.current.refreshGames('test-player');
      });

      expect(apiClient.getGames).toHaveBeenCalledWith('test-player');
      expect(result.current.savedGames).toEqual([mockGames[0]]);
    });
  });

  describe('clearCache', () => {
    it('should clear localStorage and reset state', async () => {
      const { result } = renderHook(() => useGameService());

      // Setup some initial state
      await act(async () => {
        const cachedGames = JSON.stringify(mockGames);
        mockLocalStorage.getItem.mockReturnValue(cachedGames);
        await result.current.loadGames('test-player');
      });

      expect(result.current.savedGames.length).toBeGreaterThan(0);
      expect(result.current.isLoaded).toBe(true);

      // Clear cache
      act(() => {
        result.current.clearCache();
      });

      expect(mockLocalStorage.removeItem).toHaveBeenCalledWith('savedGames');
      expect(result.current.savedGames).toEqual([]);
      expect(result.current.isLoaded).toBe(false);
      expect(result.current.error).toBeNull();
    });
  });

  describe('loading states', () => {
    it('should manage loading state correctly', async () => {
      mockLocalStorage.getItem.mockReturnValue(null);
      let resolveGetGames: (value: GameModel[]) => void;
      const getGamesPromise = new Promise<GameModel[]>(resolve => {
        resolveGetGames = resolve;
      });
      (apiClient.getGames as any).mockReturnValue(getGamesPromise);

      const { result } = renderHook(() => useGameService());

      // Start loading
      act(() => {
        result.current.loadGames('test-player');
      });

      // Should be loading
      expect(result.current.isLoading).toBe(true);
      expect(result.current.isLoaded).toBe(false);

      // Resolve the API call
      act(() => {
        resolveGetGames!(mockGames);
      });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.isLoaded).toBe(true);
      expect(result.current.savedGames).toEqual([mockGames[0], mockGames[2]]);
    });
  });
});