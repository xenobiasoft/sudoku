import { type CSSProperties, type KeyboardEvent } from 'react';
import type { CellModel } from '../types';
import { getBoxSize } from '../utils/gameUtils';
import CellInput from './CellInput';
import styles from './GameBoard.module.css';

interface GameBoardProps {
  cells: CellModel[];
  invalidCells: CellModel[];
  selectedCell: { row: number; column: number } | null;
  pencilMode: boolean;
  size?: number;
  onCellSelect: (row: number, column: number) => void;
  onKeyDown: (e: KeyboardEvent<HTMLDivElement>) => void;
}

export default function GameBoard({
  cells,
  invalidCells,
  selectedCell,
  size = 9,
  onCellSelect,
  onKeyDown,
}: GameBoardProps) {
  const boxSize = getBoxSize(size);
  const rows = Array.from({ length: size }, (_, i) => i);
  const cols = Array.from({ length: size }, (_, i) => i);

  const selectedCellData = selectedCell
    ? cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column)
    : null;
  const selectedValue = selectedCellData?.hasValue ? selectedCellData.value : null;

  const isHighlighted = (row: number, col: number): boolean => {
    if (!selectedCell) return false;
    return (
      selectedCell.row === row ||
      selectedCell.column === col ||
      (Math.floor(selectedCell.row / boxSize) === Math.floor(row / boxSize) &&
        Math.floor(selectedCell.column / boxSize) === Math.floor(col / boxSize))
    );
  };

  const isInvalid = (row: number, col: number): boolean =>
    invalidCells.some(c => c.row === row && c.column === col);

  const isSameNumber = (cell: CellModel): boolean =>
    selectedValue !== null &&
    cell.hasValue &&
    cell.value === selectedValue &&
    !(selectedCell?.row === cell.row && selectedCell?.column === cell.column);

  const boardStyle = { '--grid-size': size, '--box-size': boxSize } as CSSProperties;
  const boardClass = size === 16 ? `${styles.board} ${styles.boardLarge}` : styles.board;

  return (
    <div className={styles.boardOuter}>
      <div
        className={boardClass}
        style={boardStyle}
        tabIndex={0}
        onKeyDown={onKeyDown}
        role="grid"
      >
        {rows.map(r =>
          cols.map(c => {
            const cell = cells.find(cell => cell.row === r && cell.column === c);
            if (!cell) return <div key={`${r}-${c}`} />;
            return (
              <CellInput
                key={`${r}-${c}`}
                cell={cell}
                size={size}
                boxSize={boxSize}
                isSelected={selectedCell?.row === r && selectedCell?.column === c}
                isHighlighted={isHighlighted(r, c)}
                isSameNumber={isSameNumber(cell)}
                isInvalid={isInvalid(r, c)}
                onSelect={() => onCellSelect(r, c)}
              />
            );
          })
        )}
      </div>
    </div>
  );
}
