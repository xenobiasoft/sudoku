import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { GameModel } from '../types';
import { usePlayerService } from '../hooks/usePlayerService';
import { useGameService } from '../hooks/useGameService';
import Layout from '../components/Layout';
import SudokuImage from '../components/SudokuImage';
import GameThumbnail from '../components/GameThumbnail';
import styles from './HomePage.module.css';

export default function HomePage() {
  const navigate = useNavigate();
  const { playerAlias, isInitialized } = usePlayerService();
  const { savedGames, isLoaded: gamesLoaded, loadGames, deleteGame: deleteGameService } = useGameService();
  const [newGameOpen, setNewGameOpen] = useState(false);
  const [loadGameOpen, setLoadGameOpen] = useState(false);

  useEffect(() => {
    if (isInitialized && playerAlias) {
      loadGames(playerAlias);
    }
  }, [isInitialized, playerAlias, loadGames]);

  const handleLoadGameToggle = async () => {
    setLoadGameOpen(o => !o);
    setNewGameOpen(false);
  };

  const handleNewGameToggle = () => {
    setNewGameOpen(o => !o);
    setLoadGameOpen(false);
  };

  const handleSelectDifficulty = (difficulty: string) => {
    navigate(`/new/${difficulty}`);
  };

  const handleSelectGame = (game: GameModel) => {
    navigate(`/game/${game.id}`);
  };

  const handleDeleteGame = async (game: GameModel) => {
    if (!playerAlias) return;
    try {
      await deleteGameService(playerAlias, game.id);
    } catch (e) {
      console.error('Failed to delete game', e);
    }
  };

  return (
    <Layout>
      <div className={styles.landingPage}>
        <SudokuImage />
        <ul className={styles.menu}>
          <li>
            <button onClick={handleNewGameToggle}>
              <i className="fa fa-plus" /> Start New Game
            </button>
            <ul className={`${styles.subMenu} ${newGameOpen ? styles.subMenuOpen : ''}`}>
              {['Easy', 'Medium', 'Hard'].map(d => (
                <li key={d}>
                  <button onClick={() => handleSelectDifficulty(d)}>{d}</button>
                </li>
              ))}
            </ul>
          </li>
          <li>
            <button onClick={handleLoadGameToggle} disabled={!gamesLoaded || savedGames.length === 0}>
              <i className="fa fa-folder-open" /> Load Game
            </button>
            <ul className={`${styles.subMenu} ${loadGameOpen ? styles.subMenuOpen : ''}`}>
              {savedGames.length === 0 && loadGameOpen && gamesLoaded && (
                <li><span style={{ color: '#666', fontSize: '1rem' }}>No saved games</span></li>
              )}
              {savedGames.map(game => (
                <li key={game.id}>
                  <GameThumbnail
                    game={game}
                    onSelect={handleSelectGame}
                    onDelete={handleDeleteGame}
                  />
                </li>
              ))}
            </ul>
          </li>
        </ul>
      </div>
    </Layout>
  );
}
