import type { CellModel } from '../types';
import styles from './GameControls.module.css';

interface GameControlsProps {
  cells: CellModel[];
  pencilMode: boolean;
  canUndo: boolean;
  onNumberClick: (value: number) => void;
  onErase: () => void;
  onUndo: () => void;
  onReset: () => void;
  onTogglePencil: () => void;
}

export default function GameControls({
  cells,
  pencilMode,
  canUndo,
  onNumberClick,
  onErase,
  onUndo,
  onReset,
  onTogglePencil,
}: GameControlsProps) {
  const remainingFor = (n: number): number => {
    const placed = cells.filter(c => c.hasValue && c.value === n).length;
    return 9 - placed;
  };

  return (
    <div className={styles.controls}>
      <div className={styles.numberPad}>
        {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => {
          const remaining = remainingFor(n);
          return (
            <button
              key={n}
              type="button"
              className={styles.numBtn}
              onClick={() => onNumberClick(n)}
              disabled={remaining <= 0}
              aria-label={String(n)}
            >
              <span className={styles.numDigit}>{n}</span>
              <span className={`${styles.numRemaining} tnum`}>{remaining}</span>
            </button>
          );
        })}
      </div>

      <div className={styles.actionRow}>
        <button type="button" className={styles.actionBtn} onClick={onUndo} disabled={!canUndo}>
          <i className="fa fa-undo" />
          <span className={styles.actionLabel}>Undo</span>
        </button>
        <button type="button" className={styles.actionBtn} onClick={onErase}>
          <i className="fa fa-eraser" />
          <span className={styles.actionLabel}>Erase</span>
        </button>
        <button
          type="button"
          className={`${styles.actionBtn} ${pencilMode ? styles.actionActive : ''}`}
          onClick={onTogglePencil}
          aria-pressed={pencilMode}
        >
          <i className="fa fa-pencil" />
          <span className={styles.actionLabel}>Pencil</span>
        </button>
        <button type="button" className={styles.actionBtn} disabled title="Hints coming soon">
          <i className="fa fa-lightbulb" />
          <span className={styles.actionLabel}>Hint</span>
        </button>
        <button type="button" className={styles.actionBtn} onClick={onReset}>
          <i className="fa fa-trash-can" />
          <span className={styles.actionLabel}>Reset</span>
        </button>
      </div>
    </div>
  );
}
