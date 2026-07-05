import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { GameModel } from '../types';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import Layout from '../components/Layout';
import GameThumbnail from '../components/GameThumbnail';
import styles from './GameListPage.module.css';

export default function GameListPage() {
  const navigate = useNavigate();
  const { isNewPlayer, profileId } = usePlayerService();
  const { savedGames, isLoaded, isLoading, error, loadGames, deleteGame } = useGameService();

  useEffect(() => {
    if (isNewPlayer) { navigate('/'); return; }
    if (profileId) loadGames(profileId);
  }, [isNewPlayer, profileId, loadGames, navigate]);

  const handleSelect = (game: GameModel) => navigate(`/game/${game.id}`);

  const handleDelete = async (game: GameModel) => {
    if (!profileId) return;
    try {
      await deleteGame(profileId, game.id);
    } catch (e) {
      console.error('Failed to delete game', e);
    }
  };

  return (
    <Layout>
      <div className={styles.container}>
        <h1 className={styles.title}>Saved games</h1>

        {isLoading && <p className={styles.status}>Loading…</p>}
        {error && <p className={styles.errorStatus}>Failed to load games. Please try again.</p>}

        {isLoaded && savedGames.length === 0 && (
          <div className={styles.emptyState}>
            <p className={styles.emptyText}>nothing here yet</p>
            <button type="button" className={styles.startButton} onClick={() => navigate('/select-difficulty')}>
              Start a game
            </button>
          </div>
        )}

        {savedGames.length > 0 && (
          <ul className={styles.gameList}>
            {savedGames.map(game => (
              <li key={game.id}>
                <GameThumbnail game={game} onSelect={handleSelect} onDelete={handleDelete} />
              </li>
            ))}
          </ul>
        )}
      </div>
    </Layout>
  );
}
