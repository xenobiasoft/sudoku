import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import HomePage from './HomePage';

const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

const mockUsePlayerService = vi.fn();
vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));

const mockLoadGames = vi.fn();
const mockUseGameService = vi.fn();
vi.mock('../hooks/useGameService', () => ({
  useGameService: () => mockUseGameService(),
}));

function newPlayer() {
  return { playerAlias: null, profileId: null, isInitialized: false, isNewPlayer: true, isLoading: false, error: null, initializePlayer: vi.fn(), clearPlayer: vi.fn() };
}

function returningPlayer(alias = 'test-user') {
  return { playerAlias: alias, profileId: 'p1', isInitialized: true, isNewPlayer: false, isLoading: false, error: null, initializePlayer: vi.fn(), clearPlayer: vi.fn() };
}

function renderPage() {
  return render(<MemoryRouter><HomePage /></MemoryRouter>);
}

beforeEach(() => {
  mockNavigate.mockClear();
  mockLoadGames.mockClear();
  mockLoadGames.mockResolvedValue(undefined);
  mockUsePlayerService.mockReturnValue(returningPlayer());
  mockUseGameService.mockReturnValue({ savedGames: [], loadGames: mockLoadGames });
});

describe('HomePage', () => {
  it('renders three navigation cards', () => {
    renderPage();
    expect(screen.getByText('New game')).toBeInTheDocument();
    expect(screen.getByText('Saved games')).toBeInTheDocument();
    expect(screen.getByText('Profile')).toBeInTheDocument();
  });

  describe('new player', () => {
    beforeEach(() => { mockUsePlayerService.mockReturnValue(newPlayer()); });

    it('shows the "Create your alias" prompt on the profile card', () => {
      renderPage();
      expect(screen.getByText(/Create your alias/i)).toBeInTheDocument();
    });

    it('disables the New game card', () => {
      renderPage();
      expect(screen.getByRole('button', { name: /New game/i })).toBeDisabled();
    });

    it('disables the Saved games card', () => {
      renderPage();
      expect(screen.getByRole('button', { name: /Saved games/i })).toBeDisabled();
    });

    it('keeps the Profile card enabled', () => {
      renderPage();
      expect(screen.getByRole('button', { name: /Profile/i })).not.toBeDisabled();
    });

    it('navigates to /create-profile when Profile card is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Profile/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
    });

    it('does not load games on mount', () => {
      renderPage();
      expect(mockLoadGames).not.toHaveBeenCalled();
    });
  });

  describe('returning player', () => {
    it('shows the "Manage your alias" prompt on the profile card', () => {
      renderPage();
      expect(screen.getByText(/Manage your alias/i)).toBeInTheDocument();
    });

    it('greets the player by alias', () => {
      renderPage();
      expect(screen.getByText('test-user')).toBeInTheDocument();
    });

    it('loads saved games on mount', () => {
      renderPage();
      expect(mockLoadGames).toHaveBeenCalledWith('p1');
    });

    it('shows the saved games count', () => {
      mockUseGameService.mockReturnValue({
        savedGames: [{ id: 'a' }, { id: 'b' }],
        loadGames: mockLoadGames,
      });
      renderPage();
      expect(screen.getByText(/2 in progress/i)).toBeInTheDocument();
    });

    it('navigates to /profile when Profile card is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Profile/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/profile');
    });

    it('navigates to /select-difficulty when New game card is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /New game/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/select-difficulty');
    });

    it('navigates to /games when Saved games card is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Saved games/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/games');
    });
  });
});
