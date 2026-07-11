import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import Layout from '../components/Layout';
import styles from './HomePage.module.css';

export default function HomePage() {
  const navigate = useNavigate();
  const { isInitialized, playerAlias, profileId } = usePlayerService();
  const { savedGames, loadGames } = useGameService();

  useEffect(() => {
    if (isInitialized && profileId) {
      loadGames(profileId).catch(() => { /* ignore */ });
    }
  }, [isInitialized, profileId, loadGames]);

  const savedCount = savedGames.length;
  const savedSubtitle = !isInitialized
    ? 'None yet'
    : savedCount > 0
      ? `${savedCount} in progress`
      : 'None yet';

  return (
    <Layout>
      <div className={styles.home}>
        <div className={styles.greeting}>
          {playerAlias ? (
            <>
              <span className={styles.welcome}>welcome back,</span>
              <span className={styles.alias}>{playerAlias}</span>
            </>
          ) : (
            <span className={styles.alias}>Sudoku</span>
          )}
        </div>

        <div className={styles.cards}>
          <button
            type="button"
            className={`${styles.card} ${styles.cardPrimary}`}
            onClick={() => navigate('/select-difficulty')}
            disabled={!isInitialized}
            aria-disabled={!isInitialized}
          >
            <span className={styles.cardText}>
              <span className={styles.cardTitle}>New game</span>
              <span className={styles.cardSubtitle}>Start a fresh puzzle</span>
            </span>
            <i className={`fa-solid fa-arrow-right ${styles.arrow}`} />
          </button>

          <button
            type="button"
            className={styles.card}
            onClick={() => navigate('/games')}
            disabled={!isInitialized}
            aria-disabled={!isInitialized}
          >
            <span className={styles.cardText}>
              <span className={styles.cardTitle}>Saved games</span>
              <span className={styles.cardSubtitle}>{savedSubtitle}</span>
            </span>
            <i className={`fa-solid fa-arrow-right ${styles.arrowAccent}`} />
          </button>

          <button
            type="button"
            className={styles.card}
            onClick={() => navigate('/stats')}
            disabled={!isInitialized}
            aria-disabled={!isInitialized}
          >
            <span className={styles.cardText}>
              <span className={styles.cardTitle}>Stats</span>
              <span className={styles.cardSubtitle}>Track your progress</span>
            </span>
            <i className={`fa-solid fa-arrow-right ${styles.arrowAccent}`} />
          </button>

          <button
            type="button"
            className={styles.card}
            onClick={() => navigate(isInitialized ? '/profile' : '/create-profile')}
          >
            <span className={styles.cardText}>
              <span className={styles.cardTitle}>Profile</span>
              <span className={styles.cardSubtitle}>{isInitialized ? 'Manage your alias' : 'Create your alias'}</span>
            </span>
            <i className={`fa-solid fa-arrow-right ${styles.arrowAccent}`} />
          </button>
        </div>
      </div>
    </Layout>
  );
}
