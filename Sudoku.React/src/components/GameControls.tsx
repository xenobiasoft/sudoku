import styles from './GameControls.module.css';

interface GameControlsProps {
  pencilMode: boolean;
  canUndo: boolean;
  onNumberClick: (value: number) => void;
  onErase: () => void;
  onHome: () => void;
  onUndo: () => void;
  onReset: () => void;
  onTogglePencil: () => void;
}

export default function GameControls({
  pencilMode,
  canUndo,
  onNumberClick,
  onErase,
  onHome,
  onUndo,
  onReset,
  onTogglePencil,
}: GameControlsProps) {
  return (
    <div className={styles.btnToolbar}>
      <div className={styles.numberPanel}>
        {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => (
          <button
            key={n}
            className={styles.btnNum}
            onClick={() => onNumberClick(n)}
          >
            {n}
          </button>
        ))}
        <button className={styles.btnNum} onClick={onErase} title="Erase">
          <i className="fa fa-eraser" />
        </button>
      </div>
      <div className={styles.btnPanel}>
        <button className={styles.btn} onClick={onHome} title="Home">
          <i className="fa fa-home" />
        </button>
        <button
          className={styles.btn}
          onClick={onUndo}
          disabled={!canUndo}
          title="Undo"
        >
          <i className="fa fa-undo" />
        </button>
        <button className={styles.btn} onClick={onReset} title="Reset">
          <i className="fa fa-rotate-left" />
        </button>
        <button
          className={`${styles.btn} ${pencilMode ? '' : styles.btnOutline}`}
          onClick={onTogglePencil}
          title="Pencil mode"
        >
          <i className="fa fa-pencil" />
        </button>
      </div>
    </div>
  );
}
