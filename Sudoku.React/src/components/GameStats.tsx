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
      <div className={styles.statHeader} onClick={() => setExpanded(e => !e)}>
        <span className={styles.label}>Time</span>
        <span className={styles.value}>{formatTime(elapsedSeconds)}</span>
        <i className={`fa fa-chevron-${expanded ? 'up' : 'down'}`} />
      </div>
      {expanded && (
        <>
          <div className={styles.statItem}>
            <span className={styles.label}>Total Moves</span>
            <span className={styles.value}>{statistics.totalMoves}</span>
          </div>
          <div className={styles.statItem}>
            <span className={styles.label}>Invalid Moves</span>
            <span className={styles.value}>{statistics.invalidMoves}</span>
          </div>
        </>
      )}
    </div>
  );
}
