import { useState, useCallback, useRef } from 'react';
import { apiClient } from '../api/apiClient';
import type { PlayerStatsModel } from '../types';

export interface UseStatsServiceReturn {
  stats: PlayerStatsModel | null;
  isLoading: boolean;
  error: string | null;
  isLoaded: boolean;
  loadStats: (profileId: string) => Promise<void>;
}

export function useStatsService(): UseStatsServiceReturn {
  const [stats, setStats] = useState<PlayerStatsModel | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);
  const loadingRef = useRef(false);

  const loadStats = useCallback(async (profileId: string) => {
    if (!profileId) return;

    // Use ref to prevent concurrent calls since state updates are async
    if (loadingRef.current) return;

    loadingRef.current = true;
    setIsLoading(true);
    setError(null);

    try {
      setStats(await apiClient.getPlayerStats(profileId));
      setIsLoaded(true);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load stats';
      setError(errorMessage);
      setIsLoaded(true); // Still mark as loaded even on error
    } finally {
      loadingRef.current = false;
      setIsLoading(false);
    }
  }, []);

  return { stats, isLoading, error, isLoaded, loadStats };
}
