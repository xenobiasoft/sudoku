import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import GameThumbnail from './GameThumbnail';
import { makeGame } from '../test/helpers';

describe('GameThumbnail', () => {
  it('renders an 81-cell grid', () => {
    const game = makeGame();
    render(<GameThumbnail game={game} onSelect={() => {}} onDelete={() => {}} />);
    // 81 cell divs are rendered for the thumbnail
    const cells = document.querySelectorAll('[class*="cell"]');
    expect(cells.length).toBeGreaterThanOrEqual(81);
  });

  it('calls onSelect with the game when thumbnail is clicked', async () => {
    const user = userEvent.setup();
    const onSelect = vi.fn();
    const game = makeGame({ difficulty: 'Easy', status: 'InProgress' });
    render(<GameThumbnail game={game} onSelect={onSelect} onDelete={() => {}} />);
    await user.click(screen.getByTitle('Easy - InProgress'));
    expect(onSelect).toHaveBeenCalledWith(game);
  });

  it('calls onDelete with the game when delete button is clicked', async () => {
    const user = userEvent.setup();
    const onDelete = vi.fn();
    const game = makeGame();
    render(<GameThumbnail game={game} onSelect={() => {}} onDelete={onDelete} />);
    await user.click(screen.getByTitle('Delete game'));
    expect(onDelete).toHaveBeenCalledWith(game);
  });

  it('renders cell values from the game', () => {
    const game = makeGame();
    game.cells[0] = { ...game.cells[0], value: 8, hasValue: true };
    render(<GameThumbnail game={game} onSelect={() => {}} onDelete={() => {}} />);
    // At least one cell with value 8 should be visible
    const eights = screen.getAllByText('8');
    expect(eights.length).toBeGreaterThan(0);
  });
});
