import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import HomePage from './HomePage';
import { makeGame } from '../test/helpers';

const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

vi.mock('../api/apiClient', () => ({
  apiClient: {
    createPlayer: vi.fn().mockResolvedValue('new-alias'),
    playerExists: vi.fn().mockResolvedValue(true),
    getGames: vi.fn().mockResolvedValue([]),
    deleteGame: vi.fn().mockResolvedValue(undefined),
  },
}));

import { apiClient } from '../api/apiClient';

function renderHomePage() {
  return render(
    <MemoryRouter>
      <HomePage />
    </MemoryRouter>
  );
}

beforeEach(() => {
  mockNavigate.mockClear();
  vi.mocked(apiClient.createPlayer).mockResolvedValue('new-alias');
  vi.mocked(apiClient.playerExists).mockResolvedValue(true);
  vi.mocked(apiClient.getGames).mockResolvedValue([]);
  vi.mocked(apiClient.deleteGame).mockResolvedValue(undefined);
  localStorage.setItem('playerAlias', 'existing-player');
});

describe('HomePage', () => {
  it('renders Start New Game and Load Game buttons', () => {
    renderHomePage();
    expect(screen.getByText(/Start New Game/i)).toBeInTheDocument();
    expect(screen.getByText(/Load Game/i)).toBeInTheDocument();
  });

  it('calls playerExists for existing players', async () => {
    renderHomePage();
    await waitFor(() => {
      expect(apiClient.playerExists).toHaveBeenCalledWith('existing-player');
    });
  });

  it('creates a new player when alias is missing', async () => {
    localStorage.removeItem('playerAlias');
    renderHomePage();
    await waitFor(() => {
      expect(apiClient.createPlayer).toHaveBeenCalled();
    });
  });

  it('creates a new player when playerExists returns false', async () => {
    vi.mocked(apiClient.playerExists).mockResolvedValue(false);
    renderHomePage();
    await waitFor(() => {
      expect(apiClient.createPlayer).toHaveBeenCalled();
    });
  });

  it('shows New Game submenu when Start New Game is clicked', async () => {
    const user = userEvent.setup();
    renderHomePage();
    // The submenu ul should not have the open class initially
    const subMenus = document.querySelectorAll('ul ul');
    const newGameSubMenu = subMenus[0];
    expect(newGameSubMenu.className).not.toContain('subMenuOpen');
    await user.click(screen.getByText(/Start New Game/i));
    expect(newGameSubMenu.className).toContain('subMenuOpen');
  });

  it('navigates to /new/:difficulty when a difficulty is selected', async () => {
    const user = userEvent.setup();
    renderHomePage();
    await user.click(screen.getByText(/Start New Game/i));
    await user.click(screen.getByRole('button', { name: 'Medium' }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Medium');
  });

  it('loads games when Load Game is clicked', async () => {
    const user = userEvent.setup();
    const games = [makeGame({ id: 'g1', status: 'InProgress' })];
    vi.mocked(apiClient.getGames).mockResolvedValue(games);
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      expect(apiClient.getGames).toHaveBeenCalledWith('existing-player');
    });
  });

  it('shows "No saved games" when there are no saved games', async () => {
    const user = userEvent.setup();
    vi.mocked(apiClient.getGames).mockResolvedValue([]);
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      expect(screen.getByText(/No saved games/i)).toBeInTheDocument();
    });
  });

  it('filters out completed games from the list', async () => {
    const user = userEvent.setup();
    const games = [
      makeGame({ id: 'g1', status: 'InProgress' }),
      makeGame({ id: 'g2', status: 'Completed' }),
    ];
    vi.mocked(apiClient.getGames).mockResolvedValue(games);
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      // g1 (InProgress) should appear; g2 (Completed) should not
      // One thumbnail should be present (for g1)
      const thumbnails = document.querySelectorAll('[title="Easy - InProgress"]');
      expect(thumbnails).toHaveLength(1);
    });
  });

  it('navigates to /game/:id when a saved game is selected', async () => {
    const user = userEvent.setup();
    const game = makeGame({ id: 'game-abc', difficulty: 'Hard', status: 'InProgress' });
    vi.mocked(apiClient.getGames).mockResolvedValue([game]);
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      expect(screen.getByTitle('Hard - InProgress')).toBeInTheDocument();
    });
    await user.click(screen.getByTitle('Hard - InProgress'));
    expect(mockNavigate).toHaveBeenCalledWith('/game/game-abc');
  });

  it('deletes a game when delete button is clicked', async () => {
    const user = userEvent.setup();
    const game = makeGame({ id: 'del-game', status: 'InProgress' });
    vi.mocked(apiClient.getGames).mockResolvedValue([game]);
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      expect(screen.getByTitle('Delete game')).toBeInTheDocument();
    });
    await user.click(screen.getByTitle('Delete game'));
    await waitFor(() => {
      expect(apiClient.deleteGame).toHaveBeenCalledWith('existing-player', 'del-game');
    });
  });

  it('closes New Game submenu when Load Game is clicked', async () => {
    const user = userEvent.setup();
    renderHomePage();
    const subMenus = document.querySelectorAll('ul ul');
    const newGameSubMenu = subMenus[0];
    await user.click(screen.getByText(/Start New Game/i));
    expect(newGameSubMenu.className).toContain('subMenuOpen');
    await user.click(screen.getByText(/Load Game/i));
    expect(newGameSubMenu.className).not.toContain('subMenuOpen');
  });
});
