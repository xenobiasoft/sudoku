import { describe, it, expect } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameBoard from './GameBoard';
import { make81Cells, makeCell } from '../test/helpers';

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
