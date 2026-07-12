import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerService } from '../hooks/usePlayerService';
import Layout from '../components/Layout';
import styles from './SelectDifficultyPage.module.css';

const DIFFICULTIES = [
  { name: 'Easy', subtitle: 'A gentle warm-up', dots: '·' },
  { name: 'Medium', subtitle: 'A steady challenge', dots: '··' },
  { name: 'Hard', subtitle: 'For a clear mind', dots: '···' },
  { name: 'Expert', subtitle: 'For the deep end', dots: '····' },
] as const;

export default function SelectDifficultyPage() {
  const navigate = useNavigate();
  const { isNewPlayer } = usePlayerService();

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  return (
    <Layout title="New game">
      <div className={styles.container}>
        <h1 className={styles.title}>Select difficulty</h1>
        <p className={styles.subtitle}>how much quiet do you want?</p>
        <div className={styles.options}>
          {DIFFICULTIES.map(({ name, subtitle, dots }) => (
            <button
              key={name}
              type="button"
              className={styles.card}
              onClick={() => navigate(`/new/${name}`)}
            >
              <span className={styles.cardText}>
                <span className={styles.cardTitle}>{name}</span>
                <span className={styles.cardSubtitle}>{subtitle}</span>
              </span>
              <span className={styles.dots}>{dots}</span>
            </button>
          ))}
        </div>
      </div>
    </Layout>
  );
}
