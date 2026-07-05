import { useState } from 'react';
import type { GameStatisticsModel } from '../types';
import styles from './GameStats.module.css';

interface GameStatsProps {
  statistics: GameStatisticsModel;
  elapsedSeconds: number;
}

function formatTime(seconds: number): string {
  const h = Math.floor(seconds / 3600);
  const m = Math.floor((seconds % 3600) / 60);
  const s = seconds % 60;
  return [h, m, s].map(v => v.toString().padStart(2, '0')).join(':');
}

export default function GameStats({ statistics, elapsedSeconds }: GameStatsProps) {
  const [expanded, setExpanded] = useState(false);

  return (
    <div className={styles.gameStats}>
      <button
        type="button"
        className={styles.statHeader}
        onClick={() => setExpanded(e => !e)}
        aria-expanded={expanded}
      >
        <span className={styles.headerLabel}>Time</span>
        <span className={`${styles.headerValue} tnum`}>{formatTime(elapsedSeconds)}</span>
        <i className={`fa fa-chevron-${expanded ? 'up' : 'down'} ${styles.chevron}`} />
      </button>
      {expanded && (
        <div className={styles.statBody}>
          <div className={styles.statItem}>
            <span className={styles.label}>Total moves</span>
            <span className={`${styles.value} tnum`}>{statistics.totalMoves}</span>
          </div>
          <div className={styles.statItem}>
            <span className={styles.label}>Invalid moves</span>
            <span className={`${styles.value} tnum`}>{statistics.invalidMoves}</span>
          </div>
        </div>
      )}
    </div>
  );
}
