import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import CellInput from './CellInput';
import { makeCell } from '../test/helpers';

const baseProps = {
  isSelected: false,
  isHighlighted: false,
  isSameNumber: false,
  isInvalid: false,
  onSelect: () => {},
};

describe('CellInput - given (fixed) cell', () => {
  it('renders the cell value as a button', () => {
    const cell = makeCell({ isFixed: true, value: 7, hasValue: true });
    render(<CellInput cell={cell} {...baseProps} />);
    const button = screen.getByRole('button');
    expect(button).toHaveTextContent('7');
  });

  it('calls onSelect when clicked', async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();
    const cell = makeCell({ isFixed: true, value: 3, hasValue: true });
    render(<CellInput cell={cell} {...baseProps} onSelect={onSelect} />);
    await user.click(screen.getByRole('button'));
    expect(onSelect).toHaveBeenCalledOnce();
  });
});

describe('CellInput - editable cell', () => {
  it('renders an empty button when the cell has no value', () => {
    const cell = makeCell({ isFixed: false, value: null, hasValue: false });
    render(<CellInput cell={cell} {...baseProps} />);
    const button = screen.getByRole('button');
    expect(button).toHaveTextContent('');
  });

  it('shows the entered value', () => {
    const cell = makeCell({ isFixed: false, value: 5, hasValue: true });
    render(<CellInput cell={cell} {...baseProps} />);
    expect(screen.getByRole('button')).toHaveTextContent('5');
  });

  it('calls onSelect when clicked', async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();
    const cell = makeCell({ isFixed: false });
    render(<CellInput cell={cell} {...baseProps} onSelect={onSelect} />);
    await user.click(screen.getByRole('button'));
    expect(onSelect).toHaveBeenCalledOnce();
  });

  it('renders pencil values when possibleValues is non-empty and cell has no value', () => {
    const cell = makeCell({ isFixed: false, hasValue: false, possibleValues: [1, 3, 5] });
    render(<CellInput cell={cell} {...baseProps} />);
    expect(screen.getByText('1')).toBeInTheDocument();
    expect(screen.getByText('3')).toBeInTheDocument();
    expect(screen.getByText('5')).toBeInTheDocument();
  });

  it('does not render pencil values when the cell has a value', () => {
    const cell = makeCell({ isFixed: false, hasValue: true, value: 4, possibleValues: [1, 3] });
    render(<CellInput cell={cell} {...baseProps} />);
    expect(screen.queryByText('1')).not.toBeInTheDocument();
  });
});

describe('CellInput at size 16 (boxSize 4)', () => {
  it('renders values 10-16 as letters A-G', () => {
    const cell = makeCell({ isFixed: true, value: 16, hasValue: true });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    expect(screen.getByRole('button')).toHaveTextContent('G');
  });

  it('renders noted candidates as a non-positional wrapped row, not a fixed 16-slot grid', () => {
    const cell = makeCell({ isFixed: false, hasValue: false, possibleValues: [10, 3, 16] });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    // Only the present candidates render — sorted ascending, as symbols.
    expect(screen.getByText('3')).toBeInTheDocument();
    expect(screen.getByText('A')).toBeInTheDocument();
    expect(screen.getByText('G')).toBeInTheDocument();
    // Absent values should not render an empty placeholder slot (unlike the 9x9 positional grid).
    expect(screen.queryByText('1')).not.toBeInTheDocument();
  });

  it('places a box-right border at the end of each 4-column box, not the last column', () => {
    const cell = makeCell({ row: 0, column: 3 });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    expect(screen.getByRole('button').className).toMatch(/boxRight/);
  });

  it('places lastCol instead of boxRight on the final column', () => {
    const cell = makeCell({ row: 0, column: 15 });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    const className = screen.getByRole('button').className;
    expect(className).toMatch(/lastCol/);
    expect(className).not.toMatch(/boxRight/);
  });

  it('does not place a box-right border mid-box', () => {
    const cell = makeCell({ row: 0, column: 1 });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    expect(screen.getByRole('button').className).not.toMatch(/boxRight/);
  });

  it('places a box-bottom border at the end of each 4-row box, not the last row', () => {
    const cell = makeCell({ row: 3, column: 0 });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    expect(screen.getByRole('button').className).toMatch(/boxBottom/);
  });

  it('places lastRow instead of boxBottom on the final row', () => {
    const cell = makeCell({ row: 15, column: 0 });
    render(<CellInput cell={cell} {...baseProps} size={16} boxSize={4} />);
    const className = screen.getByRole('button').className;
    expect(className).toMatch(/lastRow/);
    expect(className).not.toMatch(/boxBottom/);
  });
});

describe('CellInput at size 9 (boxSize 3) box borders — unchanged positional behavior', () => {
  it('places a box-right border at columns 2 and 5', () => {
    [2, 5].forEach(column => {
      const cell = makeCell({ row: 0, column });
      const { unmount } = render(<CellInput cell={cell} {...baseProps} />);
      expect(screen.getByRole('button').className).toMatch(/boxRight/);
      unmount();
    });
  });

  it('places lastCol, not boxRight, at column 8', () => {
    const cell = makeCell({ row: 0, column: 8 });
    render(<CellInput cell={cell} {...baseProps} />);
    const className = screen.getByRole('button').className;
    expect(className).toMatch(/lastCol/);
    expect(className).not.toMatch(/boxRight/);
  });
});
