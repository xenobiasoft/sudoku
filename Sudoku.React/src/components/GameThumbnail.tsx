import type { GameModel } from '../types';
import styles from './GameThumbnail.module.css';

interface GameThumbnailProps {
  game: GameModel;
  onSelect: (game: GameModel) => void;
  onDelete: (game: GameModel) => void;
}

export default function GameThumbnail({ game, onSelect, onDelete }: GameThumbnailProps) {
  const rows = Array.from({ length: 9 }, (_, r) => r);
  const cols = Array.from({ length: 9 }, (_, c) => c);

  return (
    <div className={styles.savedGameCard}>
      <div
        className={styles.sudokuThumbnail}
        onClick={() => onSelect(game)}
        title={`${game.difficulty} - ${game.status}`}
      >
        {rows.map(r =>
          cols.map(c => {
            const cell = game.cells.find(cell => cell.row === r && cell.column === c);
            return (
              <div key={`${r}-${c}`} className={styles.cell}>
                {cell?.value ?? ''}
              </div>
            );
          })
        )}
      </div>
      <button
        className={styles.deleteButton}
        onClick={() => onDelete(game)}
        title="Delete game"
      >
        <i className={`fa fa-trash ${styles.delGameIcon}`} />
      </button>
    </div>
  );
}
