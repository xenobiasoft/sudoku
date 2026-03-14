import { useRef, type KeyboardEvent } from 'react';
import type { CellModel } from '../types';
import CellInput from './CellInput';
import styles from './GameBoard.module.css';

interface GameBoardProps {
  cells: CellModel[];
  invalidCells: CellModel[];
  selectedCell: { row: number; column: number } | null;
  pencilMode: boolean;
  onCellSelect: (row: number, column: number) => void;
  onKeyDown: (e: KeyboardEvent<HTMLTableElement>) => void;
}

export default function GameBoard({
  cells,
  invalidCells,
  selectedCell,
  pencilMode,
  onCellSelect,
  onKeyDown,
}: GameBoardProps) {
  const tableRef = useRef<HTMLTableElement>(null);
  const rows = Array.from({ length: 9 }, (_, i) => i);
  const cols = Array.from({ length: 9 }, (_, i) => i);

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

  return (
    <div className={styles.gameBoardContainer}>
      <table
        ref={tableRef}
        className={styles.gameBoard}
        onKeyDown={onKeyDown}
      >
        <tbody>
          {rows.map(r => (
            <tr key={r} className={styles.gridRow}>
              {cols.map(c => {
                const cell = cells.find(cell => cell.row === r && cell.column === c);
                if (!cell) return <td key={c} />;
                return (
                  <CellInput
                    key={`${r}-${c}`}
                    cell={cell}
                    isSelected={selectedCell?.row === r && selectedCell?.column === c}
                    isHighlighted={isHighlighted(r, c)}
                    isInvalid={isInvalid(r, c)}
                    pencilMode={pencilMode}
                    onSelect={() => onCellSelect(r, c)}
                  />
                );
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
