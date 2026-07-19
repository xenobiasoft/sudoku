import { useEffect, useState } from 'react';
import { useParams, useNavigate, useSearchParams, type NavigateFunction } from 'react-router-dom';
import type { GameModel } from '../types';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import { ApiError } from '../api/apiClient';
import Layout from '../components/Layout';
import styles from './NewGamePage.module.css';

const SUPPORTED_SIZES = [9, 16];

interface AttemptCreateGameParams {
  profileId: string;
  difficulty: string;
  size: number;
  createGame: (profileId: string, difficulty: string, size: number) => Promise<GameModel>;
  navigate: NavigateFunction;
  onPoolEmpty: () => void;
}

/**
 * Plain (non-hook) helper shared by the initial create-on-mount effect and the
 * manual retry button, kept outside the component so effect callbacks never
 * directly reference a memoized setState-calling closure.
 */
async function attemptCreateGame({
  profileId,
  difficulty,
  size,
  createGame,
  navigate,
  onPoolEmpty,
}: AttemptCreateGameParams): Promise<void> {
  try {
    const game = await createGame(profileId, difficulty, size);
    navigate(`/game/${game.id}`, { replace: true });
  } catch (e) {
    if (e instanceof ApiError && e.status === 503) {
      onPoolEmpty();
      return;
    }
    console.error('Failed to create game', e);
    navigate('/');
  }
}

export default function NewGamePage() {
  const { difficulty } = useParams<{ difficulty: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { profileId, isInitialized, isNewPlayer } = usePlayerService();
  const { createGame } = useGameService();
  const [poolEmpty, setPoolEmpty] = useState(false);

  const sizeParam = searchParams.get('size');
  const parsedSize = sizeParam ? parseInt(sizeParam, 10) : 9;
  const size = SUPPORTED_SIZES.includes(parsedSize) ? parsedSize : 9;

  useEffect(() => {
    if (!isInitialized || !profileId || !difficulty) {
      return;
    }
    attemptCreateGame({
      profileId,
      difficulty,
      size,
      createGame,
      navigate,
      onPoolEmpty: () => setPoolEmpty(true),
    });
  }, [difficulty, navigate, profileId, isInitialized, createGame, size]);

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  const handleRetry = () => {
    if (!profileId || !difficulty) return;
    attemptCreateGame({
      profileId,
      difficulty,
      size,
      createGame,
      navigate,
      onPoolEmpty: () => setPoolEmpty(true),
    });
  };

  if (poolEmpty) {
    return (
      <Layout hideHeader>
        <div className={styles.loadingContainer}>
          <p className={styles.message}>Preparing puzzles — try again in a moment</p>
          <button type="button" className={styles.retryButton} onClick={handleRetry}>
            Retry
          </button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout hideHeader>
      <div className={styles.loadingContainer}>
        <div className={styles.dots} role="status" aria-label="Loading puzzle">
          {[0, 1, 2].map(n => (
            <span key={n} className={styles.dot} data-testid="loader-dot" />
          ))}
        </div>
        <p className={styles.message}>setting out a {difficulty?.toLowerCase()} puzzle…</p>
      </div>
    </Layout>
  );
}
