import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import type { ProfileInfo } from '../types';

const PROFILE_KEY = 'sudoku-profile';
const LEGACY_ALIAS_KEY = 'sudoku-alias';

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

function writeProfile(info: ProfileInfo): void {
  localStorage.setItem(PROFILE_KEY, JSON.stringify(info));
}

export interface UsePlayerServiceReturn {
  playerAlias: string | null;
  profileId: string | null;
  isInitialized: boolean;
  isLoading: boolean;
  error: string | null;
  initializePlayer: () => Promise<void>;
  clearPlayer: () => void;
}

export function usePlayerService(): UsePlayerServiceReturn {
  const navigate = useNavigate();
  const clearedRef = useRef(false);
  const [playerAlias, setPlayerAlias] = useState<string | null>(null);
  const [profileId, setProfileId] = useState<string | null>(null);
  const [isInitialized, setIsInitialized] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const initializePlayer = async () => {
    if (isLoading) return;

    setIsLoading(true);
    setError(null);

    try {
      // Step 1: Check for valid sudoku-profile
      const profile = readProfile();

      if (profile) {
        // Verify profile still exists in backend (FR-9)
        const getResult = await apiClient.getProfile(profile.alias);
        if (getResult.status === 200 && getResult.data) {
          setPlayerAlias(profile.alias);
          setProfileId(profile.profileId);
          setIsInitialized(true);
          return;
        }

        if (getResult.status === 404) {
          // Orphaned profile — attempt re-create (FR-9)
          const createResult = await apiClient.createProfile(profile.alias);
          if (createResult.status === 201 && createResult.data) {
            const newInfo: ProfileInfo = { profileId: createResult.data.profileId, alias: createResult.data.alias };
            writeProfile(newInfo);
            setPlayerAlias(newInfo.alias);
            setProfileId(newInfo.profileId);
            setIsInitialized(true);
            return;
          }
          // 409 or other error — fall through to creation flow
        }

        navigate('/create-profile');
        return;
      }

      // Step 2: Check for legacy sudoku-alias (silent migration, FR-5)
      const legacyAlias = localStorage.getItem(LEGACY_ALIAS_KEY);
      if (legacyAlias) {
        const normalizedAlias = legacyAlias.trim().toLowerCase();

        // If alias is exactly 50 chars, skip suffix retry (FR-5 edge case)
        const canRetry = normalizedAlias.length < 50;

        const createResult = await apiClient.createProfile(normalizedAlias);
        if (createResult.status === 201 && createResult.data) {
          const info: ProfileInfo = { profileId: createResult.data.profileId, alias: createResult.data.alias };
          writeProfile(info);
          localStorage.removeItem(LEGACY_ALIAS_KEY);
          setPlayerAlias(info.alias);
          setProfileId(info.profileId);
          setIsInitialized(true);
          return;
        }

        if (createResult.status === 409 && canRetry) {
          const suffix = String(Math.floor(Math.random() * 90) + 10);
          const aliasWithSuffix = normalizedAlias.slice(0, 48) + suffix;
          const retryResult = await apiClient.createProfile(aliasWithSuffix);
          if (retryResult.status === 201 && retryResult.data) {
            const info: ProfileInfo = { profileId: retryResult.data.profileId, alias: retryResult.data.alias };
            writeProfile(info);
            localStorage.removeItem(LEGACY_ALIAS_KEY);
            setPlayerAlias(info.alias);
            setProfileId(info.profileId);
            setIsInitialized(true);
            return;
          }
        }

        // Migration failed — redirect to creation flow
        navigate('/create-profile');
        return;
      }

      // Step 3: No profile at all — redirect to creation flow
      navigate('/create-profile');
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
    localStorage.removeItem(PROFILE_KEY);
    localStorage.removeItem(LEGACY_ALIAS_KEY);
    setPlayerAlias(null);
    setProfileId(null);
    setIsInitialized(false);
    setError(null);
  };

  useEffect(() => {
    if (!isInitialized && !isLoading && !clearedRef.current) {
      initializePlayer();
    }
  }, [isInitialized, isLoading]);

  return {
    playerAlias,
    profileId,
    isInitialized,
    isLoading,
    error,
    initializePlayer,
    clearPlayer,
  };
}
