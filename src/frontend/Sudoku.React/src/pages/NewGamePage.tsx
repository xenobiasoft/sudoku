import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import Layout from '../components/Layout';
import styles from './NewGamePage.module.css';

export default function NewGamePage() {
  const { difficulty } = useParams<{ difficulty: string }>();
  const navigate = useNavigate();
  const { profileId, isInitialized, isNewPlayer } = usePlayerService();
  const { createGame } = useGameService();

  useEffect(() => {
    const create = async () => {
      if (!isInitialized || !profileId || !difficulty) {
        return;
      }
      try {
        const game = await createGame(profileId, difficulty);
        navigate(`/game/${game.id}`, { replace: true });
      } catch (e) {
        console.error('Failed to create game', e);
        navigate('/');
      }
    };
    create();
  }, [difficulty, navigate, profileId, isInitialized, createGame]);

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

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
