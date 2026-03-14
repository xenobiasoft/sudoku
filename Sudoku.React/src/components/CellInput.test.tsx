import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import CellInput from './CellInput';
import { makeCell } from '../test/helpers';

describe('CellInput - fixed cell', () => {
  it('renders a label with the cell value when isFixed', () => {
    const cell = makeCell({ isFixed: true, value: 7, hasValue: true });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={() => {}} />
      </tr></tbody></table>
    );
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.queryByRole('textbox')).not.toBeInTheDocument();
  });

  it('calls onSelect when fixed cell is clicked', async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();
    const cell = makeCell({ isFixed: true, value: 3, hasValue: true });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={onSelect} />
      </tr></tbody></table>
    );
    await user.click(screen.getByText('3'));
    expect(onSelect).toHaveBeenCalledOnce();
  });
});

describe('CellInput - editable cell', () => {
  it('renders an input element', () => {
    const cell = makeCell({ isFixed: false, value: null, hasValue: false });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={() => {}} />
      </tr></tbody></table>
    );
    expect(screen.getByRole('textbox')).toBeInTheDocument();
  });

  it('shows the cell value in input when hasValue is true', () => {
    const cell = makeCell({ isFixed: false, value: 5, hasValue: true });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={() => {}} />
      </tr></tbody></table>
    );
    expect(screen.getByDisplayValue('5')).toBeInTheDocument();
  });

  it('shows empty input when cell has no value', () => {
    const cell = makeCell({ isFixed: false, value: null, hasValue: false });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={() => {}} />
      </tr></tbody></table>
    );
    expect(screen.getByDisplayValue('')).toBeInTheDocument();
  });

  it('calls onSelect when input is clicked', async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();
    const cell = makeCell({ isFixed: false });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={false} onSelect={onSelect} />
      </tr></tbody></table>
    );
    await user.click(screen.getByRole('textbox'));
    expect(onSelect).toHaveBeenCalledOnce();
  });

  it('renders pencil values when possibleValues is non-empty and cell has no value', () => {
    const cell = makeCell({ isFixed: false, hasValue: false, possibleValues: [1, 3, 5] });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={true} onSelect={() => {}} />
      </tr></tbody></table>
    );
    expect(screen.getByText('1')).toBeInTheDocument();
    expect(screen.getByText('3')).toBeInTheDocument();
    expect(screen.getByText('5')).toBeInTheDocument();
  });

  it('does not render pencil values when cell has a value', () => {
    const cell = makeCell({ isFixed: false, hasValue: true, value: 4, possibleValues: [1, 3] });
    render(
      <table><tbody><tr>
        <CellInput cell={cell} isSelected={false} isHighlighted={false} isInvalid={false} pencilMode={true} onSelect={() => {}} />
      </tr></tbody></table>
    );
    // pencil area should not be shown
    expect(screen.queryByText('1')).not.toBeInTheDocument();
  });
});
