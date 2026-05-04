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
  mockUsePlayerService.mockReturnValue(returningPlayer());
});

describe('HomePage', () => {
  it('renders three navigation cards', () => {
    renderPage();
    expect(screen.getByText(/Manage Profile/i)).toBeInTheDocument();
    expect(screen.getByText(/Start New Game/i)).toBeInTheDocument();
    expect(screen.getByText(/Browse Game List/i)).toBeInTheDocument();
  });

  describe('new player', () => {
    beforeEach(() => { mockUsePlayerService.mockReturnValue(newPlayer()); });

    it('shows "Create Profile" on the profile card', () => {
      renderPage();
      expect(screen.getByText(/Create Profile/i)).toBeInTheDocument();
    });

    it('disables Start New Game card', () => {
      renderPage();
      expect(screen.getByRole('button', { name: /Start New Game/i })).toBeDisabled();
    });

    it('disables Browse Game List card', () => {
      renderPage();
      expect(screen.getByRole('button', { name: /Browse Game List/i })).toBeDisabled();
    });

    it('shows helper text on both disabled cards', () => {
      renderPage();
      expect(screen.getAllByText(/Create a profile to unlock this/i)).toHaveLength(2);
    });

    it('navigates to /create-profile when Profile card is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Create Profile/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/create-profile');
    });

    it('does not make any API calls on mount', () => {
      renderPage();
      expect(screen.queryByText(/Loading/i)).not.toBeInTheDocument();
    });
  });

  describe('returning player', () => {
    it('shows "Manage Profile" on the profile card', () => {
      renderPage();
      expect(screen.getByText(/Manage Profile/i)).toBeInTheDocument();
    });

    it('enables all three cards', () => {
      renderPage();
      screen.getAllByRole('button').forEach(btn => expect(btn).not.toBeDisabled());
    });

    it('does not show helper text', () => {
      renderPage();
      expect(screen.queryByText(/Create a profile to unlock this/i)).not.toBeInTheDocument();
    });

    it('navigates to /profile when Manage Profile is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Manage Profile/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/profile');
    });

    it('navigates to /select-difficulty when Start New Game is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Start New Game/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/select-difficulty');
    });

    it('navigates to /games when Browse Game List is clicked', async () => {
      const user = userEvent.setup();
      renderPage();
      await user.click(screen.getByRole('button', { name: /Browse Game List/i }));
      expect(mockNavigate).toHaveBeenCalledWith('/games');
    });
  });
});
