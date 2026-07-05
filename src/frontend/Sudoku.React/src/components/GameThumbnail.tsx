import type { GameModel } from '../types';
import styles from './GameThumbnail.module.css';

interface GameThumbnailProps {
  game: GameModel;
  onSelect: (game: GameModel) => void;
  onDelete: (game: GameModel) => void;
}

function formatDuration(playDuration: string | undefined): string {
  if (!playDuration) return '00:00';
  const [h = 0, m = 0, s = 0] = playDuration.split(':').map(Number);
  const pad = (v: number) => v.toString().padStart(2, '0');
  return h > 0 ? `${h}:${pad(m)}:${pad(s)}` : `${pad(m)}:${pad(s)}`;
}

export default function GameThumbnail({ game, onSelect, onDelete }: GameThumbnailProps) {
  const rows = Array.from({ length: 9 }, (_, r) => r);
  const cols = Array.from({ length: 9 }, (_, c) => c);
  const meta = `${formatDuration(game.statistics?.playDuration)} · ${game.statistics?.totalMoves ?? 0} moves`;

  return (
    <div className={styles.savedGameCard}>
      <button
        type="button"
        className={styles.sudokuThumbnail}
        onClick={() => onSelect(game)}
        title={`${game.difficulty} - ${game.status}`}
      >
        {rows.map(r =>
          cols.map(c => {
            const cell = game.cells.find(cell => cell.row === r && cell.column === c);
            const cellClass = cell?.isFixed ? `${styles.cell} ${styles.given}` : `${styles.cell} ${styles.entry}`;
            return (
              <span key={`${r}-${c}`} className={cellClass}>
                {cell?.value ?? ''}
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
