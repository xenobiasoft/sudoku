import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import Layout from '../components/Layout';
import styles from './SelectDifficultyPage.module.css';

export default function SelectDifficultyPage() {
  const navigate = useNavigate();
  const { isNewPlayer } = usePlayerService();

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  return (
    <Layout>
      <div className={styles.container}>
        <h2 className={styles.title}>Select Difficulty</h2>
        <div className={styles.options}>
          {(['Easy', 'Medium', 'Hard'] as const).map(difficulty => (
            <button
              key={difficulty}
              className={styles.difficultyButton}
              onClick={() => navigate(`/new/${difficulty}`)}
            >
              {difficulty}
            </button>
          ))}
        </div>
        <button className={styles.backButton} onClick={() => navigate('/')}>
          ← Back
        </button>
      </div>
    </Layout>
  );
}
