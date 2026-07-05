import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import userEvent from '@testing-library/user-event';
import VictoryDisplay from './VictoryDisplay';
import { makeStats } from '../test/helpers';

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

function renderVictory(overrides: Partial<React.ComponentProps<typeof VictoryDisplay>> = {}) {
  const props = {
    difficulty: 'Medium',
    statistics: makeStats({ totalMoves: 12, invalidMoves: 3 }),
    elapsedSeconds: 65,
    onClose: vi.fn(),
    ...overrides,
  };
  render(
    <MemoryRouter>
      <VictoryDisplay {...props} />
    </MemoryRouter>
  );
  return props;
}

describe('VictoryDisplay', () => {
  it('renders the "Solved" headline', () => {
    renderVictory();
    expect(screen.getByText('Solved')).toBeInTheDocument();
  });

  it('renders the difficulty eyebrow and stat values', () => {
    renderVictory();
    expect(screen.getByText('Medium')).toBeInTheDocument();
    expect(screen.getByText('01:05')).toBeInTheDocument(); // time
    expect(screen.getByText('12')).toBeInTheDocument(); // moves
    expect(screen.getByText('3')).toBeInTheDocument(); // invalid
  });

  it('renders New puzzle and Home buttons', () => {
    renderVictory();
    expect(screen.getByRole('button', { name: /new puzzle/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /home/i })).toBeInTheDocument();
  });

  it('navigates to difficulty selection when New puzzle is clicked', async () => {
    const user = userEvent.setup();
    mockNavigate.mockClear();
    renderVictory();
    await user.click(screen.getByRole('button', { name: /new puzzle/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/select-difficulty');
  });

  it('calls onClose when Home is clicked', async () => {
    const user = userEvent.setup();
    const onClose = vi.fn();
    renderVictory({ onClose });
    await user.click(screen.getByRole('button', { name: /home/i }));
    expect(onClose).toHaveBeenCalledOnce();
  });
});
