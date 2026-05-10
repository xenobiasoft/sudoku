import { useState } from 'react';
import type { ProfileInfo } from '../types';

const PROFILE_KEY = 'sudoku-profile';

function readProfile(): ProfileInfo | null {
  try {
    const raw = localStorage.getItem(PROFILE_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as ProfileInfo;
    if (!parsed?.profileId || !parsed?.alias) return null;
    return parsed;
  } catch {
    return null;
  }
}

export interface UsePlayerServiceReturn {
  playerAlias: string | null;
  profileId: string | null;
  isInitialized: boolean;
  isNewPlayer: boolean;
  isLoading: boolean;
  error: string | null;
  initializePlayer: () => Promise<void>;
  clearPlayer: () => void;
}

export function usePlayerService(): UsePlayerServiceReturn {
  const [profile, setProfile] = useState<ProfileInfo | null>(() => {
    return readProfile();
  });

  const clearPlayer = () => {
    try { localStorage.removeItem(PROFILE_KEY); } catch { /* ignore */ }
    setProfile(null);
  };

  return {
    playerAlias: profile?.alias ?? null,
    profileId: profile?.profileId ?? null,
    isInitialized: profile !== null,
    isNewPlayer: profile === null,
    isLoading: false,
    error: null,
    initializePlayer: async () => {},
    clearPlayer,
  };
}
