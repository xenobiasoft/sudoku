import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameStats from './GameStats';
import { makeStats } from '../test/helpers';

describe('GameStats', () => {
  it('renders the time label', () => {
    render(<GameStats statistics={makeStats()} elapsedSeconds={0} />);
    expect(screen.getByText('Time')).toBeInTheDocument();
  });

  it('formats elapsed seconds as HH:MM:SS', () => {
    render(<GameStats statistics={makeStats()} elapsedSeconds={3661} />);
    expect(screen.getByText('01:01:01')).toBeInTheDocument();
  });

  it('formats 0 seconds as 00:00:00', () => {
    render(<GameStats statistics={makeStats()} elapsedSeconds={0} />);
    expect(screen.getByText('00:00:00')).toBeInTheDocument();
  });

  it('does not show expanded stats by default', () => {
    render(<GameStats statistics={makeStats({ totalMoves: 5 })} elapsedSeconds={0} />);
    expect(screen.queryByText('Total Moves')).not.toBeInTheDocument();
  });

  it('shows expanded stats after clicking the header', async () => {
    const user = userEvent.setup();
    render(<GameStats statistics={makeStats({ totalMoves: 7, invalidMoves: 2 })} elapsedSeconds={0} />);
    await user.click(screen.getByText('Time'));
    expect(screen.getByText('Total Moves')).toBeInTheDocument();
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.getByText('Invalid Moves')).toBeInTheDocument();
    expect(screen.getByText('2')).toBeInTheDocument();
  });

  it('collapses expanded stats on second click', async () => {
    const user = userEvent.setup();
    render(<GameStats statistics={makeStats({ totalMoves: 3 })} elapsedSeconds={0} />);
    const header = screen.getByText('Time');
    await user.click(header);
    expect(screen.getByText('Total Moves')).toBeInTheDocument();
    await user.click(header);
    expect(screen.queryByText('Total Moves')).not.toBeInTheDocument();
  });
});
