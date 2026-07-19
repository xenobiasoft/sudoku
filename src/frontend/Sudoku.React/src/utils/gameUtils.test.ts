import { describe, it, expect } from 'vitest';
import {
  getCell,
  getRowCells,
  getColumnCells,
  getMiniGridCells,
  isSolved,
  validateCells,
  deriveSize,
  getBoxSize,
} from './gameUtils';
import { makeCell, make81Cells, makeCells } from '../test/helpers';

describe('getCell', () => {
  it('returns the matching cell', () => {
    const cells = [makeCell({ row: 1, column: 3, value: 5, hasValue: true })];
    expect(getCell(cells, 1, 3)?.value).toBe(5);
  });

  it('returns undefined when cell not found', () => {
    expect(getCell([], 0, 0)).toBeUndefined();
  });
});

describe('getRowCells', () => {
  it('returns only cells in the given row', () => {
    const cells = [
      makeCell({ row: 0, column: 0 }),
      makeCell({ row: 0, column: 1 }),
      makeCell({ row: 1, column: 0 }),
    ];
    const row0 = getRowCells(cells, 0);
    expect(row0).toHaveLength(2);
    expect(row0.every(c => c.row === 0)).toBe(true);
  });
});

describe('getColumnCells', () => {
  it('returns only cells in the given column', () => {
    const cells = [
      makeCell({ row: 0, column: 2 }),
      makeCell({ row: 1, column: 2 }),
      makeCell({ row: 0, column: 0 }),
    ];
    const col2 = getColumnCells(cells, 2);
    expect(col2).toHaveLength(2);
    expect(col2.every(c => c.column === 2)).toBe(true);
  });
});

describe('getMiniGridCells', () => {
  it('returns 9 cells for a 3x3 block', () => {
    const cells = make81Cells();
    const block = getMiniGridCells(cells, 0, 0);
    expect(block).toHaveLength(9);
    block.forEach(c => {
      expect(c.row).toBeGreaterThanOrEqual(0);
      expect(c.row).toBeLessThan(3);
      expect(c.column).toBeGreaterThanOrEqual(0);
      expect(c.column).toBeLessThan(3);
    });
  });

  it('returns cells for the middle 3x3 block', () => {
    const cells = make81Cells();
    const block = getMiniGridCells(cells, 4, 4);
    expect(block).toHaveLength(9);
    block.forEach(c => {
      expect(c.row).toBeGreaterThanOrEqual(3);
      expect(c.row).toBeLessThan(6);
      expect(c.column).toBeGreaterThanOrEqual(3);
      expect(c.column).toBeLessThan(6);
    });
  });
});

describe('validateCells', () => {
  it('returns empty array when no duplicates', () => {
    const cells = make81Cells();
    expect(validateCells(cells)).toHaveLength(0);
  });

  it('flags duplicate values in the same row', () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, value: 5, hasValue: true });
    cells[1] = makeCell({ row: 0, column: 1, value: 5, hasValue: true });
    const invalid = validateCells(cells);
    expect(invalid.some(c => c.row === 0 && c.column === 0)).toBe(true);
    expect(invalid.some(c => c.row === 0 && c.column === 1)).toBe(true);
  });

  it('flags duplicate values in the same column', () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, value: 3, hasValue: true });
    cells[9] = makeCell({ row: 1, column: 0, value: 3, hasValue: true });
    const invalid = validateCells(cells);
    expect(invalid.some(c => c.row === 0 && c.column === 0)).toBe(true);
    expect(invalid.some(c => c.row === 1 && c.column === 0)).toBe(true);
  });

  it('flags duplicate values in the same 3x3 block', () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, value: 7, hasValue: true });
    cells[11] = makeCell({ row: 1, column: 2, value: 7, hasValue: true });
    const invalid = validateCells(cells);
    expect(invalid.some(c => c.row === 0 && c.column === 0)).toBe(true);
    expect(invalid.some(c => c.row === 1 && c.column === 2)).toBe(true);
  });

  it('does not flag cells without values', () => {
    const cells = make81Cells();
    // all cells have no value — no conflicts
    expect(validateCells(cells)).toHaveLength(0);
  });
});

describe('isSolved', () => {
  it('returns false when cells count is not 81', () => {
    expect(isSolved([])).toBe(false);
    expect(isSolved([makeCell()])).toBe(false);
  });

  it('returns false when some cells have no value', () => {
    const cells = make81Cells();
    expect(isSolved(cells)).toBe(false);
  });

  it('returns false when all cells have values but there are duplicates', () => {
    const cells = make81Cells();
    cells.forEach(c => {
      c.hasValue = true;
      c.value = 1; // all same value — many duplicates
    });
    expect(isSolved(cells)).toBe(false);
  });

  it('returns true when all cells have values and no duplicates', () => {
    // Build a valid solved sudoku board
    const solution = [
      [5,3,4,6,7,8,9,1,2],
      [6,7,2,1,9,5,3,4,8],
      [1,9,8,3,4,2,5,6,7],
      [8,5,9,7,6,1,4,2,3],
      [4,2,6,8,5,3,7,9,1],
      [7,1,3,9,2,4,8,5,6],
      [9,6,1,5,3,7,2,8,4],
      [2,8,7,4,1,9,6,3,5],
      [3,4,5,2,8,6,1,7,9],
    ];
    const cells: ReturnType<typeof makeCell>[] = [];
    for (let r = 0; r < 9; r++) {
      for (let c = 0; c < 9; c++) {
        cells.push(makeCell({ row: r, column: c, value: solution[r][c], hasValue: true, isFixed: true }));
      }
    }
    expect(isSolved(cells)).toBe(true);
  });
});

describe('deriveSize', () => {
  it('derives 9 from 81 cells', () => {
    expect(deriveSize(make81Cells())).toBe(9);
  });

  it('derives 16 from 256 cells', () => {
    expect(deriveSize(makeCells(16))).toBe(16);
  });
});

describe('getBoxSize', () => {
  it('derives 3 for a 9x9 board', () => {
    expect(getBoxSize(9)).toBe(3);
  });

  it('derives 4 for a 16x16 board', () => {
    expect(getBoxSize(16)).toBe(4);
  });
});

describe('16x16 (boxSize 4) behavior', () => {
  it('getMiniGridCells returns 16 cells for a 4x4 block', () => {
    const cells = makeCells(16);
    const block = getMiniGridCells(cells, 0, 0);
    expect(block).toHaveLength(16);
    block.forEach(c => {
      expect(c.row).toBeGreaterThanOrEqual(0);
      expect(c.row).toBeLessThan(4);
      expect(c.column).toBeGreaterThanOrEqual(0);
      expect(c.column).toBeLessThan(4);
    });
  });

  it('getMiniGridCells returns cells for an inner 4x4 block', () => {
    const cells = makeCells(16);
    const block = getMiniGridCells(cells, 9, 9);
    expect(block).toHaveLength(16);
    block.forEach(c => {
      expect(c.row).toBeGreaterThanOrEqual(8);
      expect(c.row).toBeLessThan(12);
      expect(c.column).toBeGreaterThanOrEqual(8);
      expect(c.column).toBeLessThan(12);
    });
  });

  it('validateCells flags duplicates within a 4x4 box on a 16x16 board', () => {
    const cells = makeCells(16);
    cells[0] = makeCell({ row: 0, column: 0, value: 11, hasValue: true });
    cells[find16Index(3, 3)] = makeCell({ row: 3, column: 3, value: 11, hasValue: true });
    const invalid = validateCells(cells);
    expect(invalid.some(c => c.row === 0 && c.column === 0)).toBe(true);
    expect(invalid.some(c => c.row === 3 && c.column === 3)).toBe(true);
  });

  it('validateCells does not flag the same value in different rows/columns/boxes on a 16x16 board', () => {
    const cells = makeCells(16);
    cells[find16Index(0, 0)] = makeCell({ row: 0, column: 0, value: 16, hasValue: true });
    cells[find16Index(4, 4)] = makeCell({ row: 4, column: 4, value: 16, hasValue: true });
    const invalid = validateCells(cells);
    expect(invalid).toHaveLength(0);
  });

  it('isSolved rejects a 16x16-shaped cell collection with the wrong cell count', () => {
    // One cell short of 256 — must be rejected by the cell-count guard itself,
    // not merely because the board happens to be unsolved.
    expect(isSolved(makeCells(16).slice(0, 255))).toBe(false);
  });

  function find16Index(row: number, col: number): number {
    return row * 16 + col;
  }
});
