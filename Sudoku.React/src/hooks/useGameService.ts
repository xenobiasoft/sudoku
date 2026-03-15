import { useState, useCallback, useRef } from 'react';
import { apiClient } from '../api/apiClient';
import type { GameModel } from '../types';

export interface UseGameServiceReturn {
  // Saved games collection
  savedGames: GameModel[];
  isLoading: boolean;
  error: string | null;
  isLoaded: boolean;
  loadGames: (playerAlias: string, forceRefresh?: boolean) => Promise<void>;
  deleteGame: (playerAlias: string, gameId: string) => Promise<void>;
  createGame: (playerAlias: string, difficulty: string) => Promise<GameModel>;
  clearCache: () => void;
  refreshGames: (playerAlias: string) => Promise<void>;
  
  // Single game management
  currentGame: GameModel | null;
  isGameLoading: boolean;
  gameError: string | null;
  getGame: (playerAlias: string, gameId: string) => Promise<GameModel>;
  updateStatus: (playerAlias: string, gameId: string, status: string) => Promise<void>;
  makeMove: (playerAlias: string, gameId: string, row: number, column: number, value: number | null, duration: string) => Promise<GameModel>;
  undoMove: (playerAlias: string, gameId: string) => Promise<GameModel>;
  resetGame: (playerAlias: string, gameId: string) => Promise<GameModel>;
  addPossibleValue: (playerAlias: string, gameId: string, row: number, column: number, value: number) => Promise<GameModel>;
  removePossibleValue: (playerAlias: string, gameId: string, row: number, column: number, value: number) => Promise<GameModel>;
  clearPossibleValues: (playerAlias: string, gameId: string, row: number, column: number) => Promise<GameModel>;
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

  const loadGames = useCallback(async (playerAlias: string, forceRefresh = false) => {
    if (!playerAlias) return;
    
    // Use ref to prevent concurrent calls since state updates are async
    if (loadingRef.current) return;
    
    loadingRef.current = true;
    setIsLoading(true);
    setError(null);

    try {
      // First check localStorage if not forcing refresh
      if (!forceRefresh) {
        const cachedGames = localStorage.getItem('savedGames');
        if (cachedGames) {
          try {
            const parsedGames: GameModel[] = JSON.parse(cachedGames);
            // Filter out completed games
            const availableGames = parsedGames.filter(g => g.status !== 'Completed');
            setSavedGames(availableGames);
            setIsLoaded(true);
            setIsLoading(false);
            return;
          } catch {
            // Invalid cached data, remove it and continue with API call
            localStorage.removeItem('savedGames');
          }
        }
      }

      // Fetch from API
      const games = await apiClient.getGames(playerAlias);
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

  const deleteGame = useCallback(async (playerAlias: string, gameId: string) => {
    if (!playerAlias || loadingRef.current) return;

    setError(null);

    try {
      // Delete from API
      await apiClient.deleteGame(playerAlias, gameId);

      // Update local state
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
          // If cache is corrupted, just remove it
          localStorage.removeItem('savedGames');
        }
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete game';
      setError(errorMessage);
      console.error('Failed to delete game:', err);
      throw err; // Re-throw so calling component can handle it
    }
  }, [isLoading]);

  const refreshGames = useCallback(async (playerAlias: string) => {
    await loadGames(playerAlias, true);
  }, [loadGames]);

  const createGame = useCallback(async (playerAlias: string, difficulty: string): Promise<GameModel> => {
    if (!playerAlias) {
      throw new Error('Player alias is required');
    }

    setError(null);

    try {
      debugger
      // Create the game via API
      const newGame = await apiClient.createGame(playerAlias, difficulty);

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

  const getGame = useCallback(async (playerAlias: string, gameId: string): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setIsGameLoading(true);
    setGameError(null);

    try {
      const game = await apiClient.getGame(playerAlias, gameId);
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

  const updateStatus = useCallback(async (playerAlias: string, gameId: string, status: string): Promise<void> => {
    if (!playerAlias || !gameId) return;

    try {
      await apiClient.updateStatus(playerAlias, gameId, status);
    } catch (err) {
      console.error('Failed to update status:', err);
      // Don't throw here as this is often called in cleanup scenarios
    }
  }, []);

  const makeMove = useCallback(async (playerAlias: string, gameId: string, row: number, column: number, value: number | null, duration: string): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.makeMove(playerAlias, gameId, row, column, value, duration);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to make move';
      setGameError(errorMessage);
      console.error('Failed to make move:', err);
      throw err;
    }
  }, []);

  const undoMove = useCallback(async (playerAlias: string, gameId: string): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.undoMove(playerAlias, gameId);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to undo move';
      setGameError(errorMessage);
      console.error('Failed to undo move:', err);
      throw err;
    }
  }, []);

  const resetGame = useCallback(async (playerAlias: string, gameId: string): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.resetGame(playerAlias, gameId);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to reset game';
      setGameError(errorMessage);
      console.error('Failed to reset game:', err);
      throw err;
    }
  }, []);

  const addPossibleValue = useCallback(async (playerAlias: string, gameId: string, row: number, column: number, value: number): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.addPossibleValue(playerAlias, gameId, row, column, value);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to add possible value';
      setGameError(errorMessage);
      console.error('Failed to add possible value:', err);
      throw err;
    }
  }, []);

  const removePossibleValue = useCallback(async (playerAlias: string, gameId: string, row: number, column: number, value: number): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.removePossibleValue(playerAlias, gameId, row, column, value);
      setCurrentGame(updatedGame);
      return updatedGame;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to remove possible value';
      setGameError(errorMessage);
      console.error('Failed to remove possible value:', err);
      throw err;
    }
  }, []);

  const clearPossibleValues = useCallback(async (playerAlias: string, gameId: string, row: number, column: number): Promise<GameModel> => {
    if (!playerAlias || !gameId) {
      throw new Error('Player alias and game ID are required');
    }

    setGameError(null);

    try {
      const updatedGame = await apiClient.clearPossibleValues(playerAlias, gameId, row, column);
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
    updateStatus,
    makeMove,
    undoMove,
    resetGame,
    addPossibleValue,
    removePossibleValue,
    clearPossibleValues,
    clearCurrentGame,
  };
}