import { useState, useCallback, useRef } from 'react';
import { apiClient } from '../api/apiClient';
import type { GameModel } from '../types';

export interface UseGameServiceReturn {
  // Saved games collection
  savedGames: GameModel[];
  isLoading: boolean;
  error: string | null;
  isLoaded: boolean;
  loadGames: (profileId: string, forceRefresh?: boolean) => Promise<void>;
  deleteGame: (profileId: string, gameId: string) => Promise<void>;
  createGame: (profileId: string, difficulty: string) => Promise<GameModel>;
  clearCache: () => void;
  refreshGames: (profileId: string) => Promise<void>;

  // Single game management
  currentGame: GameModel | null;
  isGameLoading: boolean;
  gameError: string | null;
  getGame: (profileId: string, gameId: string) => Promise<GameModel>;
  pauseGame: (profileId: string, gameId: string) => Promise<void>;
  resumeGame: (profileId: string, gameId: string) => Promise<void>;
  makeMove: (profileId: string, gameId: string, row: number, column: number, value: number | null, duration: string) => Promise<GameModel>;
  undoMove: (profileId: string, gameId: string) => Promise<GameModel>;
  requestHint: (profileId: string, gameId: string, duration: string) => Promise<GameModel>;
  resetGame: (profileId: string, gameId: string) => Promise<GameModel>;
  addPossibleValue: (profileId: string, gameId: string, row: number, column: number, value: number) => Promise<GameModel>;
  removePossibleValue: (profileId: string, gameId: string, row: number, column: number, value: number) => Promise<GameModel>;
  clearPossibleValues: (profileId: string, gameId: string, row: number, column: number) => Promise<GameModel>;
  clearCurrentGame: () => void;
}

export function useGameService(): UseGameServiceReturn {
  // Saved games collection state
  const [savedGames, setSavedGames] = useState<GameModel[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const loadingRef = useRef(false);

  // Current game state
  const [currentGame, setCurrentGame] = useState<GameModel | null>(null);
  const [isGameLoading, setIsGameLoading] = useState(false);
  const [gameError, setGameError] = useState<string | null>(null);

  const clearCache = useCallback(() => {
    localStorage.removeItem('savedGames');
    setSavedGames([]);
    setIsLoaded(false);
    setError(null);
  }, []);

  const loadGames = useCallback(async (profileId: string, forceRefresh = false) => {
    if (!profileId) return;

    // Use ref to prevent concurrent calls since state updates are async
    if (loadingRef.current) return;

    loadingRef.current = true;
    setIsLoading(true);
    setError(null);

    try {
      // Stale-while-revalidate: paint cached games immediately (if any) for a fast
      // first render, then always fetch from the API below to refresh with the
      // authoritative server state (move count, play duration, difficulty, etc.).
      if (!forceRefresh) {
        const cachedGames = localStorage.getItem('savedGames');
        if (cachedGames) {
          try {
            const parsedGames: GameModel[] = JSON.parse(cachedGames);
            if (parsedGames && parsedGames.length > 0) {
              const availableGames = parsedGames.filter(g => g.status !== 'Completed');
              setSavedGames(availableGames);
              setIsLoaded(true);
            }
          } catch {
            localStorage.removeItem('savedGames');
          }
        }
      }

      // Fetch from API
      const games = await apiClient.getGames(profileId);
      const availableGames = games.filter(g => g.status !== 'Completed');

      // Cache the full games array (including completed games for future use)
      localStorage.setItem('savedGames', JSON.stringify(games));

      setSavedGames(availableGames);
      setIsLoaded(true);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load games';
      setError(errorMessage);
      console.error('Failed to load games:', err);
      setIsLoaded(true); // Still mark as loaded even on error
    } finally {
      loadingRef.current = false;
      setIsLoading(false);
    }
  }, []);

  const deleteGame = useCallback(async (profileId: string, gameId: string) => {
    if (!profileId || loadingRef.current) return;

    setError(null);

    try {
      // Delete from API
      await apiClient.deleteGame(profileId, gameId);
    } catch (err) {
      if (!(err instanceof Error) || !err.message.startsWith('HTTP 404')) {
        const errorMessage = err instanceof Error ? err.message : 'Failed to delete game';
        setError(errorMessage);
        console.error('Failed to delete game:', err);
        throw err;
      }
    }

    // Update local state (runs on success or 404 — game is gone either way)
    setSavedGames(games => games.filter(g => g.id !== gameId));

    // Clear current game if it's the one being deleted
    setCurrentGame(current => current?.id === gameId ? null : current);

    // Update localStorage cache
    const cachedGames = localStorage.getItem('savedGames');
    if (cachedGames) {
      try {
        const parsedGames: GameModel[] = JSON.parse(cachedGames);
        const updatedGames = parsedGames.filter(g => g.id !== gameId);
        localStorage.setItem('savedGames', JSON.stringify(updatedGames));
      } catch {
        localStorage.removeItem('savedGames');
      }
    }
  }, [isLoading]);

  const refreshGames = useCallback(async (profileId: string) => {
    await loadGames(profileId, true);
  }, [loadGames]);

  const createGame = useCallback(async (profileId: string, difficulty: string): Promise<GameModel> => {
    if (!profileId) {
      throw new Error('Profile ID is required');
    }

    setError(null);

    try {
      // Create the game via API
      const newGame = await apiClient.createGame(profileId, difficulty);

      // Update localStorage cache with the new game
      const cachedGames = localStorage.getItem('savedGames');
      let updatedGames: GameModel[];

      if (cachedGames) {
        try {
          const parsedGames: GameModel[] = JSON.parse(cachedGames);
          updatedGames = [...parsedGames, newGame];
        } catch {
          // If cache is corrupted, start fresh with just the new game
          updatedGames = [newGame];
        }
      } else {
        updatedGames = [newGame];
      }

      localStorage.setItem('savedGames', JSON.stringify(updatedGames));

      // Update local state (only if the new game is not completed)
      if (newGame.status !== 'Completed') {
        setSavedGames(games => [...games, newGame]);
      }

      return newGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to create game';
      setError(errorMessage);
      console.error('Failed to create game:', err);
      throw err; // Re-throw so calling component can handle it
    }
  }, []);

  // Single game management functions
  const clearCurrentGame = useCallback(() => {
    setCurrentGame(null);
    setGameError(null);
  }, []);

  const getGame = useCallback(async (profileId: string, gameId: string): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setIsGameLoading(true);
    setGameError(null);

    try {
      const game = await apiClient.getGame(profileId, gameId);
      setCurrentGame(game);
      return game;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load game';
      setGameError(errorMessage);
      console.error('Failed to load game:', err);
      throw err;
    } finally {
      setIsGameLoading(false);
    }
  }, []);

  const pauseGame = useCallback(async (profileId: string, gameId: string): Promise<void> => {
    if (!profileId || !gameId) return;
    try {
      await apiClient.pauseGame(profileId, gameId);
    } catch (err) {
      console.error('Failed to pause game:', err);
    }
  }, []);

  const resumeGame = useCallback(async (profileId: string, gameId: string): Promise<void> => {
    if (!profileId || !gameId) return;
    try {
      await apiClient.resumeGame(profileId, gameId);
    } catch (err) {
      console.error('Failed to resume game:', err);
    }
  }, []);

  const makeMove = useCallback(async (profileId: string, gameId: string, row: number, column: number, value: number | null, duration: string): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.makeMove(profileId, gameId, row, column, value, duration);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to make move';
      setGameError(errorMessage);
      console.error('Failed to make move:', err);
      throw err;
    }
  }, []);

  const undoMove = useCallback(async (profileId: string, gameId: string): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.undoMove(profileId, gameId);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to undo move';
      setGameError(errorMessage);
      console.error('Failed to undo move:', err);
      throw err;
    }
  }, []);

  const requestHint = useCallback(async (profileId: string, gameId: string, duration: string): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.getHint(profileId, gameId, duration);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to get hint';
      setGameError(errorMessage);
      console.error('Failed to get hint:', err);
      throw err;
    }
  }, []);

  const resetGame = useCallback(async (profileId: string, gameId: string): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.resetGame(profileId, gameId);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to reset game';
      setGameError(errorMessage);
      console.error('Failed to reset game:', err);
      throw err;
    }
  }, []);

  const addPossibleValue = useCallback(async (profileId: string, gameId: string, row: number, column: number, value: number): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.addPossibleValue(profileId, gameId, row, column, value);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to add possible value';
      setGameError(errorMessage);
      console.error('Failed to add possible value:', err);
      throw err;
    }
  }, []);

  const removePossibleValue = useCallback(async (profileId: string, gameId: string, row: number, column: number, value: number): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.removePossibleValue(profileId, gameId, row, column, value);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to remove possible value';
      setGameError(errorMessage);
      console.error('Failed to remove possible value:', err);
      throw err;
    }
  }, []);

  const clearPossibleValues = useCallback(async (profileId: string, gameId: string, row: number, column: number): Promise<GameModel> => {
    if (!profileId || !gameId) {
      throw new Error('Profile ID and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.clearPossibleValues(profileId, gameId, row, column);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to clear possible values';
      setGameError(errorMessage);
      console.error('Failed to clear possible values:', err);
      throw err;
    }
  }, []);

  return {
    // Saved games collection
    savedGames,
    isLoading,
    error,
    isLoaded,
    loadGames,
    deleteGame,
    createGame,
    clearCache,
    refreshGames,

    // Single game management
    currentGame,
    isGameLoading,
    gameError,
    getGame,
    pauseGame,
    resumeGame,
    makeMove,
    undoMove,
    requestHint,
    resetGame,
    addPossibleValue,
    removePossibleValue,
    clearPossibleValues,
    clearCurrentGame,
  };
}
