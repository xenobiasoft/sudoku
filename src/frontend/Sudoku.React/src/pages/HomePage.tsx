import { useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import Layout from '../components/Layout';
import SudokuImage from '../components/SudokuImage';
import styles from './HomePage.module.css';

export default function HomePage() {
  const navigate = useNavigate();
  const { isInitialized } = usePlayerService();

  return (
    <Layout>
      <div className={styles.landingPage}>
        <SudokuImage />
        <div className={styles.cards}>
          <button
            className={`${styles.card} ${!isInitialized ? styles.cardPrimary : ''}`}
            onClick={() => navigate(isInitialized ? '/profile' : '/create-profile')}
          >
            <span><i className="fa-solid fa-user" /> {isInitialized ? 'Manage Profile' : 'Create Profile'}</span>
          </button>

          <button
            className={`${styles.card} ${!isInitialized ? styles.cardDisabled : ''}`}
            onClick={() => navigate('/select-difficulty')}
            disabled={!isInitialized}
            aria-disabled={!isInitialized}
          >
            <span><i className="fa-solid fa-play-circle" /> Start New Game</span>
            {!isInitialized && <span className={styles.helperText}>Create a profile to unlock this</span>}
          </button>

          <button
            className={`${styles.card} ${!isInitialized ? styles.cardDisabled : ''}`}
            onClick={() => navigate('/games')}
            disabled={!isInitialized}
            aria-disabled={!isInitialized}
          >
            <span><i className="fa-solid fa-folder-open" /> Browse Game List</span>
            {!isInitialized && <span className={styles.helperText}>Create a profile to unlock this</span>}
          </button>
        </div>
      </div>
    </Layout>
  );
}
