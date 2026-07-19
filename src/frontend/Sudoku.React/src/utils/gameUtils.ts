import type { CellModel } from '../types';

/** Supported board cell counts (9x9 = 81, 16x16 = 256). */
const SUPPORTED_CELL_COUNTS = [81, 256];

export function getCell(cells: CellModel[], row: number, col: number): CellModel | undefined {
  return cells.find(c => c.row === row && c.column === col);
}

export function getRowCells(cells: CellModel[], row: number): CellModel[] {
  return cells.filter(c => c.row === row);
}

export function getColumnCells(cells: CellModel[], col: number): CellModel[] {
  return cells.filter(c => c.column === col);
}

/**
 * Derives the board size (9 or 16) from a cell collection. This is the documented
 * fallback for cached game state predating the `size` field on `GameModel`.
 */
export function deriveSize(cells: CellModel[]): number {
  return Math.sqrt(cells.length);
}

/** Derives the box size (3 for 9x9, 4 for 16x16) from the board size. */
export function getBoxSize(size: number): number {
  return Math.sqrt(size);
}

export function getMiniGridCells(cells: CellModel[], row: number, col: number): CellModel[] {
  const size = deriveSize(cells);
  const boxSize = getBoxSize(size);
  const startRow = Math.floor(row / boxSize) * boxSize;
  const startCol = Math.floor(col / boxSize) * boxSize;
  return cells.filter(
    c => c.row >= startRow && c.row < startRow + boxSize && c.column >= startCol && c.column < startCol + boxSize
  );
}

export function isSolved(cells: CellModel[]): boolean {
  if (!SUPPORTED_CELL_COUNTS.includes(cells.length)) return false;
  return cells.every(c => c.hasValue) && validateCells(cells).length === 0;
}

export function validateCells(cells: CellModel[]): CellModel[] {
  const invalid = new Set<string>();
  const size = deriveSize(cells);
  const boxSize = getBoxSize(size);

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

  for (let i = 0; i < size; i++) {
    checkGroup(getRowCells(cells, i));
    checkGroup(getColumnCells(cells, i));
  }

  for (let r = 0; r < size; r += boxSize) {
    for (let c = 0; c < size; c += boxSize) {
      checkGroup(getMiniGridCells(cells, r, c));
    }
  }

  return cells.filter(c => invalid.has(`${c.row},${c.column}`));
}
