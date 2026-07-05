import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameControls from './GameControls';
import { make81Cells } from '../test/helpers';

function renderControls(overrides: Partial<React.ComponentProps<typeof GameControls>> = {}) {
  const defaults = {
    cells: make81Cells(),
    pencilMode: false,
    canUndo: true,
    hintsRemaining: 3,
    onNumberClick: vi.fn(),
    onErase: vi.fn(),
    onUndo: vi.fn(),
    onHint: vi.fn(),
    onReset: vi.fn(),
    onTogglePencil: vi.fn(),
  };
  return { ...defaults, ...render(<GameControls {...defaults} {...overrides} />) };
}

describe('GameControls', () => {
  it('renders number buttons 1-9', () => {
    renderControls();
    for (let n = 1; n <= 9; n++) {
      expect(screen.getByRole('button', { name: n.toString() })).toBeInTheDocument();
    }
  });

  it('renders the action buttons', () => {
    renderControls();
    expect(screen.getByRole('button', { name: /erase/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /undo/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /reset/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /pencil/i })).toBeInTheDocument();
  });

  it('renders an enabled Hint button showing remaining hints', () => {
    renderControls({ hintsRemaining: 3 });
    const hintButton = screen.getByRole('button', { name: /hint/i });
    expect(hintButton).toBeEnabled();
    expect(hintButton).toHaveTextContent('3');
  });

  it('disables the Hint button when no hints remain', () => {
    renderControls({ hintsRemaining: 0 });
    expect(screen.getByRole('button', { name: /hint/i })).toBeDisabled();
  });

  it('calls onHint when the hint button is clicked', async () => {
    const user = userEvent.setup();
    const onHint = vi.fn();
    renderControls({ onHint, hintsRemaining: 2 });
    await user.click(screen.getByRole('button', { name: /hint/i }));
    expect(onHint).toHaveBeenCalledOnce();
  });

  it('calls onNumberClick with the correct number', async () => {
    const user = userEvent.setup();
    const onNumberClick = vi.fn();
    renderControls({ onNumberClick });
    await user.click(screen.getByRole('button', { name: '5' }));
    expect(onNumberClick).toHaveBeenCalledWith(5);
  });

  it('shows the remaining count for a number', () => {
    // A board with three 5s placed leaves 6 remaining.
    const cells = make81Cells();
    [0, 1, 2].forEach(i => {
      cells[i] = { ...cells[i], value: 5, hasValue: true };
    });
    renderControls({ cells });
    const fiveButton = screen.getByRole('button', { name: '5' });
    expect(fiveButton).toHaveTextContent('6');
  });

  it('calls onErase when erase button is clicked', async () => {
    const user = userEvent.setup();
    const onErase = vi.fn();
    renderControls({ onErase });
    await user.click(screen.getByRole('button', { name: /erase/i }));
    expect(onErase).toHaveBeenCalledOnce();
  });

  it('calls onUndo when undo button is clicked', async () => {
    const user = userEvent.setup();
    const onUndo = vi.fn();
    renderControls({ onUndo, canUndo: true });
    await user.click(screen.getByRole('button', { name: /undo/i }));
    expect(onUndo).toHaveBeenCalledOnce();
  });

  it('disables undo button when canUndo is false', () => {
    renderControls({ canUndo: false });
    expect(screen.getByRole('button', { name: /undo/i })).toBeDisabled();
  });

  it('calls onReset when reset button is clicked', async () => {
    const user = userEvent.setup();
    const onReset = vi.fn();
    renderControls({ onReset });
    await user.click(screen.getByRole('button', { name: /reset/i }));
    expect(onReset).toHaveBeenCalledOnce();
  });

  it('calls onTogglePencil when pencil button is clicked', async () => {
    const user = userEvent.setup();
    const onTogglePencil = vi.fn();
    renderControls({ onTogglePencil });
    await user.click(screen.getByRole('button', { name: /pencil/i }));
    expect(onTogglePencil).toHaveBeenCalledOnce();
  });
});
