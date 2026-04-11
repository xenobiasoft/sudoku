import type { CellModel } from '../types';

export function getCell(cells: CellModel[], row: number, col: number): CellModel | undefined {
  return cells.find(c => c.row === row && c.column === col);
}

export function getRowCells(cells: CellModel[], row: number): CellModel[] {
  return cells.filter(c => c.row === row);
}

export function getColumnCells(cells: CellModel[], col: number): CellModel[] {
  return cells.filter(c => c.column === col);
}

export function getMiniGridCells(cells: CellModel[], row: number, col: number): CellModel[] {
  const startRow = Math.floor(row / 3) * 3;
  const startCol = Math.floor(col / 3) * 3;
  return cells.filter(
    c => c.row >= startRow && c.row < startRow + 3 && c.column >= startCol && c.column < startCol + 3
  );
}

export function isSolved(cells: CellModel[]): boolean {
  if (cells.length !== 81) return false;
  return cells.every(c => c.hasValue) && validateCells(cells).length === 0;
}

export function validateCells(cells: CellModel[]): CellModel[] {
  const invalid = new Set<string>();

  const checkGroup = (group: CellModel[]) => {
    const withValues = group.filter(c => c.hasValue && c.value !== null);
    withValues.forEach(cell => {
      const dups = withValues.filter(c => c.value === cell.value && (c.row !== cell.row || c.column !== cell.column));
      if (dups.length > 0) {
        invalid.add(`${cell.row},${cell.column}`);
        dups.forEach(d => invalid.add(`${d.row},${d.column}`));
      }
    });
  };

  for (let i = 0; i < 9; i++) {
    checkGroup(getRowCells(cells, i));
    checkGroup(getColumnCells(cells, i));
  }

  for (let r = 0; r < 9; r += 3) {
    for (let c = 0; c < 9; c += 3) {
      checkGroup(getMiniGridCells(cells, r, c));
    }
  }

  return cells.filter(c => invalid.has(`${c.row},${c.column}`));
}
