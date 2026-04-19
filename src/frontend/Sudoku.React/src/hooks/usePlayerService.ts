import { useState, useEffect, useRef } from 'react';
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
  const clearedRef = useRef(false);
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
        alias = await apiClient.createPlayer();
        localStorage.setItem(playerAliasKey, alias);
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
    clearedRef.current = true;
    localStorage.removeItem(playerAliasKey);
    setPlayerAlias(null);
    setIsInitialized(false);
    setError(null);
  };

  // Auto-initialize on mount
  useEffect(() => {
    if (!isInitialized && !isLoading && !clearedRef.current) {
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