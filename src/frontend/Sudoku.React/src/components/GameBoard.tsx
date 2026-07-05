import { type KeyboardEvent } from 'react';
import type { CellModel } from '../types';
import CellInput from './CellInput';
import styles from './GameBoard.module.css';

interface GameBoardProps {
  cells: CellModel[];
  invalidCells: CellModel[];
  selectedCell: { row: number; column: number } | null;
  pencilMode: boolean;
  onCellSelect: (row: number, column: number) => void;
  onKeyDown: (e: KeyboardEvent<HTMLDivElement>) => void;
}

export default function GameBoard({
  cells,
  invalidCells,
  selectedCell,
  onCellSelect,
  onKeyDown,
}: GameBoardProps) {
  const rows = Array.from({ length: 9 }, (_, i) => i);
  const cols = Array.from({ length: 9 }, (_, i) => i);

  const selectedCellData = selectedCell
    ? cells.find(c => c.row === selectedCell.row && c.column === selectedCell.column)
    : null;
  const selectedValue = selectedCellData?.hasValue ? selectedCellData.value : null;

  const isHighlighted = (row: number, col: number): boolean => {
    if (!selectedCell) return false;
    return (
      selectedCell.row === row ||
      selectedCell.column === col ||
      (Math.floor(selectedCell.row / 3) === Math.floor(row / 3) &&
        Math.floor(selectedCell.column / 3) === Math.floor(col / 3))
    );
  };

  const isInvalid = (row: number, col: number): boolean =>
    invalidCells.some(c => c.row === row && c.column === col);

  const isSameNumber = (cell: CellModel): boolean =>
    selectedValue !== null &&
    cell.hasValue &&
    cell.value === selectedValue &&
    !(selectedCell?.row === cell.row && selectedCell?.column === cell.column);

  return (
    <div className={styles.boardOuter}>
      <div className={styles.board} tabIndex={0} onKeyDown={onKeyDown} role="grid">
        {rows.map(r =>
          cols.map(c => {
            const cell = cells.find(cell => cell.row === r && cell.column === c);
            if (!cell) return <div key={`${r}-${c}`} />;
            return (
              <CellInput
                key={`${r}-${c}`}
                cell={cell}
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
