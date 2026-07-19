import type { CSSProperties } from 'react';
import type { GameModel } from '../types';
import { formatDuration } from '../utils/timeUtils';
import { deriveSize } from '../utils/gameUtils';
import { valueToSymbol } from '../utils/symbols';
import styles from './GameThumbnail.module.css';

interface GameThumbnailProps {
  game: GameModel;
  onSelect: (game: GameModel) => void;
  onDelete: (game: GameModel) => void;
}

export default function GameThumbnail({ game, onSelect, onDelete }: GameThumbnailProps) {
  const size = game.size ?? deriveSize(game.cells);
  const rows = Array.from({ length: size }, (_, r) => r);
  const cols = Array.from({ length: size }, (_, c) => c);
  const meta = `${formatDuration(game.statistics?.playDuration, '00:00')} · ${game.statistics?.totalMoves ?? 0} moves`;
  const thumbnailStyle = { '--grid-size': size } as CSSProperties;
  const thumbnailClass = size === 16
    ? `${styles.sudokuThumbnail} ${styles.sudokuThumbnailLarge}`
    : styles.sudokuThumbnail;

  return (
    <div className={styles.savedGameCard}>
      <button
        type="button"
        className={thumbnailClass}
        style={thumbnailStyle}
        onClick={() => onSelect(game)}
        title={`${game.difficulty} - ${game.status}`}
      >
        {rows.map(r =>
          cols.map(c => {
            const cell = game.cells.find(cell => cell.row === r && cell.column === c);
            const cellClass = cell?.isFixed ? `${styles.cell} ${styles.given}` : `${styles.cell} ${styles.entry}`;
            return (
              <span key={`${r}-${c}`} className={cellClass}>
                {cell?.value != null ? valueToSymbol(cell.value) : ''}
              </span>
            );
          })
        )}
      </button>

      <div className={styles.info} onClick={() => onSelect(game)}>
        <span className={styles.difficulty}>{game.difficulty}</span>
        <span className={`${styles.meta} tnum`}>{meta}</span>
      </div>

      <button
        type="button"
        className={styles.deleteButton}
        onClick={() => onDelete(game)}
        aria-label="Delete game"
        title="Delete game"
      >
        <i className="fa-solid fa-xmark" />
      </button>
    </div>
  );
}
