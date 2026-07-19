import type { CellModel } from '../types';
import { valueToSymbol } from '../utils/symbols';
import styles from './CellInput.module.css';

interface CellInputProps {
  cell: CellModel;
  isSelected: boolean;
  isHighlighted: boolean;
  isSameNumber: boolean;
  isInvalid: boolean;
  size?: number;
  boxSize?: number;
  onSelect: () => void;
}

export default function CellInput({
  cell,
  isSelected,
  isHighlighted,
  isSameNumber,
  isInvalid,
  size = 9,
  boxSize = 3,
  onSelect,
}: CellInputProps) {
  const getCellClass = (): string => {
    const classes = [styles.cell];

    // Box separators / outer edges, derived from boxSize
    const isBoxRightCol = (cell.column + 1) % boxSize === 0 && cell.column !== size - 1;
    const isBoxBottomRow = (cell.row + 1) % boxSize === 0 && cell.row !== size - 1;
    if (isBoxRightCol) classes.push(styles.boxRight);
    if (cell.column === size - 1) classes.push(styles.lastCol);
    if (isBoxBottomRow) classes.push(styles.boxBottom);
    if (cell.row === size - 1) classes.push(styles.lastRow);

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
      : cell.isHint
        ? styles.hint
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
          {valueToSymbol(cell.value)}
        </span>
      ) : showPencil ? (
        size === 16 ? (
          <span className={styles.pencilValuesWrapped}>
            {[...cell.possibleValues]
              .sort((a, b) => a - b)
              .map(n => (
                <span key={n} className={styles.pencilEntry}>
                  {valueToSymbol(n)}
                </span>
              ))}
          </span>
        ) : (
          <span className={styles.pencilValues}>
            {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => (
              <span key={n} className={styles.pencilEntry}>
                {cell.possibleValues.includes(n) ? valueToSymbol(n) : ''}
              </span>
            ))}
          </span>
        )
      ) : null}
    </button>
  );
}
