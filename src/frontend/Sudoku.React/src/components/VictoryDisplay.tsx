import { useNavigate } from 'react-router-dom';
import type { GameStatisticsModel } from '../types';
import styles from './VictoryDisplay.module.css';

interface VictoryDisplayProps {
  difficulty: string;
  statistics?: GameStatisticsModel;
  elapsedSeconds: number;
  onClose: () => void;
}

function formatTime(seconds: number): string {
  const h = Math.floor(seconds / 3600);
  const m = Math.floor((seconds % 3600) / 60);
  const s = seconds % 60;
  const parts = h > 0 ? [h, m, s] : [m, s];
  return parts.map(v => v.toString().padStart(2, '0')).join(':');
}

export default function VictoryDisplay({ difficulty, statistics, elapsedSeconds, onClose }: VictoryDisplayProps) {
  const navigate = useNavigate();

  return (
    <div className={styles.victory}>
      {difficulty && <div className={styles.eyebrow}>{difficulty}</div>}
      <h1 className={styles.headline}>Solved</h1>
      <p className={styles.tagline}>a clear, quiet finish</p>

      <div className={styles.stats}>
        <div className={styles.stat}>
          <div className={`${styles.statValue} tnum`}>{formatTime(elapsedSeconds)}</div>
          <div className={styles.statCaption}>Time</div>
        </div>
        <div className={styles.stat}>
          <div className={`${styles.statValue} tnum`}>{statistics?.totalMoves ?? 0}</div>
          <div className={styles.statCaption}>Moves</div>
        </div>
        <div className={styles.stat}>
          <div className={`${styles.statValue} tnum`}>{statistics?.invalidMoves ?? 0}</div>
          <div className={styles.statCaption}>Invalid</div>
        </div>
      </div>

      <div className={styles.actions}>
        <button type="button" className={styles.primary} onClick={() => navigate('/select-difficulty')}>
          New puzzle
        </button>
        <button type="button" className={styles.ghost} onClick={onClose}>
          Home
        </button>
      </div>
    </div>
  );
}
