import styles from './SudokuImage.module.css';

export default function SudokuImage() {
  return (
    <div className={styles.gameImageContainer}>
      <img src="/images/sudoku_image.jpg" alt="Sudoku" />
    </div>
  );
}
