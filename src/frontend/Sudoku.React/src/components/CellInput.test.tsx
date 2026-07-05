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
