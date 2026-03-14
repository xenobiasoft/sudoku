import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { GameModel } from '../types';
import { apiClient } from '../api/apiClient';
import Layout from '../components/Layout';
import SudokuImage from '../components/SudokuImage';
import GameThumbnail from '../components/GameThumbnail';
import styles from './HomePage.module.css';

export default function HomePage() {
  const navigate = useNavigate();
  const [newGameOpen, setNewGameOpen] = useState(false);
  const [loadGameOpen, setLoadGameOpen] = useState(false);
  const [savedGames, setSavedGames] = useState<GameModel[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    initPlayer();
  }, []);

  const initPlayer = async () => {
    let alias = localStorage.getItem('playerAlias');
    if (!alias) {
      alias = await apiClient.createPlayer();
      localStorage.setItem('playerAlias', alias);
    } else {
      try {
        const exists = await apiClient.playerExists(alias);
        if (!exists) {
          alias = await apiClient.createPlayer();
          localStorage.setItem('playerAlias', alias);
        }
      } catch {
        alias = await apiClient.createPlayer();
        localStorage.setItem('playerAlias', alias);
      }
    }
  };

  const handleLoadGameToggle = async () => {
    if (!loadGameOpen) {
      setLoading(true);
      try {
        const alias = localStorage.getItem('playerAlias');
        if (alias) {
          const games = await apiClient.getGames(alias);
          setSavedGames(games.filter(g => g.status !== 'Completed'));
        }
      } catch (e) {
        console.error('Failed to load games', e);
      } finally {
        setLoading(false);
      }
    }
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
    const alias = localStorage.getItem('playerAlias');
    if (!alias) return;
    try {
      await apiClient.deleteGame(alias, game.id);
      setSavedGames(games => games.filter(g => g.id !== game.id));
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
            <button onClick={handleLoadGameToggle} disabled={loading}>
              <i className="fa fa-folder-open" /> Load Game
            </button>
            <ul className={`${styles.subMenu} ${loadGameOpen ? styles.subMenuOpen : ''}`}>
              {savedGames.length === 0 && loadGameOpen && !loading && (
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
