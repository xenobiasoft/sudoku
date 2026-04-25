import { useRef } from 'react';
import type { CellModel } from '../types';
import styles from './CellInput.module.css';

interface CellInputProps {
  cell: CellModel;
  isSelected: boolean;
  isHighlighted: boolean;
  isInvalid: boolean;
  pencilMode: boolean;
  onSelect: () => void;
}

export default function CellInput({
  cell,
  isSelected,
  isHighlighted,
  isInvalid,
  onSelect,
}: CellInputProps) {
  const inputRef = useRef<HTMLInputElement>(null);

  const getCellClass = (): string => {
    const classes = [styles.cell];
    if (isInvalid) classes.push(styles.invalid);
    else if (isSelected) classes.push(styles.selected);
    else if (isHighlighted) classes.push(styles.highlight);
    return classes.join(' ');
  };

  const handleClick = () => {
    onSelect();
    inputRef.current?.focus();
  };

  if (cell.isFixed) {
    return (
      <td className={getCellClass()}>
        <label onClick={handleClick}>{cell.value ?? ''}</label>
      </td>
    );
  }

  return (
    <td className={getCellClass()}>
      {cell.possibleValues.length > 0 && !cell.hasValue && (
        <div className={styles.pencilValues}>
          {[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => (
            <div key={n} className={styles.pencilEntry}>
              {cell.possibleValues.includes(n) ? n : ''}
            </div>
          ))}
        </div>
      )}
      <input
        ref={inputRef}
        type="text"
        readOnly
        value={cell.hasValue && cell.value !== null ? cell.value.toString() : ''}
        onClick={handleClick}
        onChange={() => {}}
        style={cell.possibleValues.length > 0 && !cell.hasValue ? { backgroundColor: 'transparent' } : undefined}
      />
    </td>
  );
}
