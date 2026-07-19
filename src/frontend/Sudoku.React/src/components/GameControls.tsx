import type { CellModel } from '../types';
import { valueToSymbol, valuesForSize } from '../utils/symbols';
import styles from './GameControls.module.css';

interface GameControlsProps {
  cells: CellModel[];
  pencilMode: boolean;
  canUndo: boolean;
  hintsRemaining: number;
  size?: number;
  onNumberClick: (value: number) => void;
  onErase: () => void;
  onUndo: () => void;
  onHint: () => void;
  onReset: () => void;
  onTogglePencil: () => void;
}

export default function GameControls({
  cells,
  pencilMode,
  canUndo,
  hintsRemaining,
  size = 9,
  onNumberClick,
  onErase,
  onUndo,
  onHint,
  onReset,
  onTogglePencil,
}: GameControlsProps) {
  const remainingFor = (n: number): number => {
    const placed = cells.filter(c => c.hasValue && c.value === n).length;
    return size - placed;
  };

  const numberPadClass = size === 16 ? styles.numberPad16 : styles.numberPad;

  return (
    <div className={styles.controls}>
      <div className={numberPadClass}>
        {valuesForSize(size).map(n => {
          const remaining = remainingFor(n);
          const symbol = valueToSymbol(n);
          return (
            <button
              key={n}
              type="button"
              className={styles.numBtn}
              onClick={() => onNumberClick(n)}
              disabled={remaining <= 0}
              aria-label={symbol}
            >
              <span className={styles.numDigit}>{symbol}</span>
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
        <button
          type="button"
          className={styles.actionBtn}
          onClick={onHint}
          disabled={hintsRemaining <= 0}
          title={hintsRemaining > 0 ? `Reveal a cell (${hintsRemaining} left)` : 'No hints remaining'}
        >
          <i className="fa fa-lightbulb" />
          <span className={styles.actionLabel}>Hint</span>
          <span className={`${styles.hintBadge} tnum`}>{hintsRemaining}</span>
        </button>
        <button type="button" className={styles.actionBtn} onClick={onReset}>
          <i className="fa fa-trash-can" />
          <span className={styles.actionLabel}>Reset</span>
        </button>
      </div>
    </div>
  );
}
