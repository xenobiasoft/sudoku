import { useState, useEffect } from 'react';
import { apiClient } from '../api/apiClient';

export interface UsePlayerServiceReturn {
  playerAlias: string | null;
  isInitialized: boolean;
  isLoading: boolean;
  error: string | null;
  initializePlayer: () => Promise<void>;
  clearPlayer: () => void;
}

export function usePlayerService(): UsePlayerServiceReturn {
  const playerAliasKey = 'sudoku-alias';
  const [playerAlias, setPlayerAlias] = useState<string | null>(null);
  const [isInitialized, setIsInitialized] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const initializePlayer = async () => {
    if (isLoading) return;
    
    setIsLoading(true);
    setError(null);
    
    try {
      let alias = localStorage.getItem(playerAliasKey);
      
      if (!alias) {
        // No stored alias, create new player
        alias = await apiClient.createPlayer();
        localStorage.setItem(playerAliasKey, alias);
      } else {
        // Check if stored alias still exists on server
        try {
          const exists = await apiClient.playerExists(alias);
          if (!exists) {
            // Player no longer exists, create new one
            alias = await apiClient.createPlayer();
            localStorage.setItem(playerAliasKey, alias);
          }
        } catch {
          // API call failed, create new player
          alias = await apiClient.createPlayer();
          localStorage.setItem(playerAliasKey, alias);
        }
      }
      
      setPlayerAlias(alias);
      setIsInitialized(true);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to initialize player';
      setError(errorMessage);
      console.error('Failed to initialize player:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const clearPlayer = () => {
    localStorage.removeItem(playerAliasKey);
    setPlayerAlias(null);
    setIsInitialized(false);
    setError(null);
  };

  // Auto-initialize on mount
  useEffect(() => {
    if (!isInitialized && !isLoading) {
      initializePlayer();
    }
  }, [isInitialized, isLoading]);

  return {
    playerAlias,
    isInitialized,
    isLoading,
    error,
    initializePlayer,
    clearPlayer,
  };
}