import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameControls from './GameControls';

function renderControls(overrides: Partial<React.ComponentProps<typeof GameControls>> = {}) {
  const defaults = {
    pencilMode: false,
    canUndo: true,
    onNumberClick: vi.fn(),
    onErase: vi.fn(),
    onHome: vi.fn(),
    onUndo: vi.fn(),
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

  it('renders the erase button', () => {
    renderControls();
    expect(screen.getByTitle('Erase')).toBeInTheDocument();
  });

  it('renders home, undo, reset, and pencil buttons', () => {
    renderControls();
    expect(screen.getByTitle('Home')).toBeInTheDocument();
    expect(screen.getByTitle('Undo')).toBeInTheDocument();
    expect(screen.getByTitle('Reset')).toBeInTheDocument();
    expect(screen.getByTitle('Pencil mode')).toBeInTheDocument();
  });

  it('calls onNumberClick with the correct number', async () => {
    const user = userEvent.setup();
    const onNumberClick = vi.fn();
    renderControls({ onNumberClick });
    await user.click(screen.getByRole('button', { name: '5' }));
    expect(onNumberClick).toHaveBeenCalledWith(5);
  });

  it('calls onErase when erase button is clicked', async () => {
    const user = userEvent.setup();
    const onErase = vi.fn();
    renderControls({ onErase });
    await user.click(screen.getByTitle('Erase'));
    expect(onErase).toHaveBeenCalledOnce();
  });

  it('calls onHome when home button is clicked', async () => {
    const user = userEvent.setup();
    const onHome = vi.fn();
    renderControls({ onHome });
    await user.click(screen.getByTitle('Home'));
    expect(onHome).toHaveBeenCalledOnce();
  });

  it('calls onUndo when undo button is clicked', async () => {
    const user = userEvent.setup();
    const onUndo = vi.fn();
    renderControls({ onUndo, canUndo: true });
    await user.click(screen.getByTitle('Undo'));
    expect(onUndo).toHaveBeenCalledOnce();
  });

  it('disables undo button when canUndo is false', () => {
    renderControls({ canUndo: false });
    expect(screen.getByTitle('Undo')).toBeDisabled();
  });

  it('calls onReset when reset button is clicked', async () => {
    const user = userEvent.setup();
    const onReset = vi.fn();
    renderControls({ onReset });
    await user.click(screen.getByTitle('Reset'));
    expect(onReset).toHaveBeenCalledOnce();
  });

  it('calls onTogglePencil when pencil button is clicked', async () => {
    const user = userEvent.setup();
    const onTogglePencil = vi.fn();
    renderControls({ onTogglePencil });
    await user.click(screen.getByTitle('Pencil mode'));
    expect(onTogglePencil).toHaveBeenCalledOnce();
  });
});
