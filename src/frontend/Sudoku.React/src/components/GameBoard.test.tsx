import { describe, it, expect } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameBoard from './GameBoard';
import { make81Cells, makeCell, makeCells } from '../test/helpers';

function renderBoard(overrides: Partial<React.ComponentProps<typeof GameBoard>> = {}) {
  const defaults = {
    cells: make81Cells(),
    invalidCells: [],
    selectedCell: null,
    pencilMode: false,
    onCellSelect: vi.fn(),
    onKeyDown: vi.fn(),
  };
  return { ...defaults, ...render(<GameBoard {...defaults} {...overrides} />) };
}

describe('GameBoard', () => {
  it('renders 81 cell buttons', () => {
    renderBoard();
    expect(screen.getAllByRole('button')).toHaveLength(81);
  });

  it('renders cell values', () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, value: 6, hasValue: true, isFixed: true });
    renderBoard({ cells });
    expect(screen.getByText('6')).toBeInTheDocument();
  });

  it('calls onCellSelect when a cell is clicked', async () => {
    const user = userEvent.setup();
    const onCellSelect = vi.fn();
    const cells = make81Cells();
    cells[4] = makeCell({ row: 0, column: 4, value: null, isFixed: false });
    renderBoard({ cells, onCellSelect });
    // Click the 5th cell of row 0 (index 4)
    const buttons = screen.getAllByRole('button');
    await user.click(buttons[4]);
    expect(onCellSelect).toHaveBeenCalledWith(0, 4);
  });

  it('fires onKeyDown when a key is pressed on the grid', () => {
    const onKeyDown = vi.fn();
    renderBoard({ onKeyDown });
    fireEvent.keyDown(screen.getByRole('grid'), { key: '5' });
    expect(onKeyDown).toHaveBeenCalled();
  });
});

describe('GameBoard - pencil mark highlighting', () => {
  const boardWithNotes = () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, value: 5, hasValue: true, isFixed: true });
    cells[40] = makeCell({ row: 4, column: 4, possibleValues: [2, 5] });
    return cells;
  };

  it('emphasizes candidates matching the selected cell value', () => {
    const { container } = renderBoard({
      cells: boardWithNotes(),
      selectedCell: { row: 0, column: 0 },
    });
    const matches = container.querySelectorAll('[class*="pencilMatch"]');
    expect(matches).toHaveLength(1);
    expect(matches[0]).toHaveTextContent('5');
  });

  it('emphasizes no candidates when the selected cell is empty', () => {
    const { container } = renderBoard({
      cells: boardWithNotes(),
      selectedCell: { row: 4, column: 4 },
    });
    expect(container.querySelectorAll('[class*="pencilMatch"]')).toHaveLength(0);
  });
});

describe('GameBoard at size 16', () => {
  it('renders 256 cell buttons', () => {
    renderBoard({ cells: makeCells(16), size: 16 });
    expect(screen.getAllByRole('button')).toHaveLength(256);
  });

  it('renders letter-symbol cell values', () => {
    const cells = makeCells(16);
    cells[0] = makeCell({ row: 0, column: 0, value: 16, hasValue: true, isFixed: true });
    renderBoard({ cells, size: 16 });
    expect(screen.getByText('G')).toBeInTheDocument();
  });
});
