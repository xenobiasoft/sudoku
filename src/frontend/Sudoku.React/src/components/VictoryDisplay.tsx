import styles from './VictoryDisplay.module.css';

interface VictoryDisplayProps {
  onClose: () => void;
}

export default function VictoryDisplay({ onClose }: VictoryDisplayProps) {
  return (
    <div className={styles.victoryOverlay}>
      <div className={styles.victoryModal}>
        <div className={styles.victoryTitle}>🎉 Puzzle Solved!</div>
        <div className={styles.victorySubtitle}>Congratulations! You completed the Sudoku puzzle.</div>
        <button className={styles.victoryButton} onClick={onClose}>
          Back to Home
        </button>
      </div>
    </div>
  );
}
