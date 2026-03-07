import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiClient } from '../api/apiClient';
import Layout from '../components/Layout';
import styles from './NewGamePage.module.css';

export default function NewGamePage() {
  const { difficulty } = useParams<{ difficulty: string }>();
  const navigate = useNavigate();

  useEffect(() => {
    const create = async () => {
      const alias = localStorage.getItem('playerAlias');
      if (!alias || !difficulty) {
        navigate('/');
        return;
      }
      try {
        const game = await apiClient.createGame(alias, difficulty);
        navigate(`/game/${game.id}`, { replace: true });
      } catch (e) {
        console.error('Failed to create game', e);
        navigate('/');
      }
    };
    create();
  }, [difficulty, navigate]);

  return (
    <Layout>
      <div className={styles.loadingContainer}>
        <h2>Creating {difficulty} puzzle...</h2>
        <div className={styles.sudokuLoader}>
          {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => (
            <span key={n}>{n}</span>
          ))}
        </div>
      </div>
    </Layout>
  );
}
