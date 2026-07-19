import { useEffect, useState } from 'react';
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
  const [selectedSize, setSelectedSize] = useState<9 | 16>(9);

  useEffect(() => {
    if (isNewPlayer) navigate('/');
  }, [isNewPlayer, navigate]);

  return (
    <Layout title="New game">
      <div className={styles.container}>
        <h1 className={styles.title}>Select difficulty</h1>
        <p className={styles.subtitle}>how much quiet do you want?</p>

        <div className={styles.sizeToggle} role="group" aria-label="Board size">
          <button
            type="button"
            className={`${styles.sizeOption} ${selectedSize === 9 ? styles.sizeOptionActive : ''}`}
            aria-pressed={selectedSize === 9}
            onClick={() => setSelectedSize(9)}
          >
            Classic 9×9
          </button>
          <button
            type="button"
            className={`${styles.sizeOption} ${selectedSize === 16 ? styles.sizeOptionActive : ''}`}
            aria-pressed={selectedSize === 16}
            onClick={() => setSelectedSize(16)}
          >
            Giant 16×16
          </button>
        </div>

        <div className={styles.options}>
          {DIFFICULTIES.map(({ name, subtitle, dots }) => (
            <button
              key={name}
              type="button"
              className={styles.card}
              onClick={() => navigate(`/new/${name}?size=${selectedSize}`)}
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
