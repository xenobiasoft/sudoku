import type { CellModel } from '../types';
import styles from './CellInput.module.css';

interface CellInputProps {
  cell: CellModel;
  isSelected: boolean;
  isHighlighted: boolean;
  isSameNumber: boolean;
  isInvalid: boolean;
  onSelect: () => void;
}

export default function CellInput({
  cell,
  isSelected,
  isHighlighted,
  isSameNumber,
  isInvalid,
  onSelect,
}: CellInputProps) {
  const getCellClass = (): string => {
    const classes = [styles.cell];

    // 3x3 box separators / outer edges
    if (cell.column === 2 || cell.column === 5) classes.push(styles.boxRight);
    if (cell.column === 8) classes.push(styles.lastCol);
    if (cell.row === 2 || cell.row === 5) classes.push(styles.boxBottom);
    if (cell.row === 8) classes.push(styles.lastRow);

    // Background fill precedence: selected > invalid > same-number > peer highlight
    if (isSelected) classes.push(styles.selected);
    else if (isInvalid) classes.push(styles.invalid);
    else if (isSameNumber) classes.push(styles.same);
    else if (isHighlighted) classes.push(styles.highlight);

    return classes.join(' ');
  };

  const digitClass = isInvalid
    ? styles.invalidDigit
    : cell.isFixed
      ? styles.given
      : styles.entry;

  const showPencil = cell.possibleValues.length > 0 && !cell.hasValue;

  return (
    <button
      type="button"
      className={getCellClass()}
      onClick={onSelect}
      aria-label={`Row ${cell.row + 1}, column ${cell.column + 1}`}
    >
      {cell.hasValue && cell.value !== null ? (
        <span key={cell.value} className={`${styles.digit} ${digitClass} tnum`}>
          {cell.value}
        </span>
      ) : showPencil ? (
        <span className={styles.pencilValues}>
          {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => (
            <span key={n} className={styles.pencilEntry}>
              {cell.possibleValues.includes(n) ? n : ''}
            </span>
          ))}
        </span>
      ) : null}
    </button>
  );
}
