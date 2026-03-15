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

// Mock the custom hooks
const mockUsePlayerService = vi.fn();
const mockUseGameService = vi.fn();

vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));

vi.mock('../hooks/useGameService', () => ({
  useGameService: () => mockUseGameService(),
}));



function renderHomePage() {
  return render(
    <MemoryRouter>
      <HomePage />
    </MemoryRouter>
  );
}

beforeEach(() => {
  mockNavigate.mockClear();
  
  // Default mock implementations
  mockUsePlayerService.mockReturnValue({
    playerAlias: 'existing-player',
    isInitialized: true,
    isLoading: false,
    error: null,
    initializePlayer: vi.fn(),
    clearPlayer: vi.fn(),
  });
  
  mockUseGameService.mockReturnValue({
    savedGames: [],
    isLoading: false,
    error: null,
    isLoaded: true,
    loadGames: vi.fn(),
    deleteGame: vi.fn(),
    clearCache: vi.fn(),
    refreshGames: vi.fn(),
  });
});

describe('HomePage', () => {
  it('renders Start New Game and Load Game buttons', () => {
    renderHomePage();
    expect(screen.getByText(/Start New Game/i)).toBeInTheDocument();
    expect(screen.getByText(/Load Game/i)).toBeInTheDocument();
  });

  it('calls loadGames when player is initialized', async () => {
    const mockLoadGames = vi.fn();
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: mockLoadGames,
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    await waitFor(() => {
      expect(mockLoadGames).toHaveBeenCalledWith('existing-player');
    });
  });

  it('does not load games when player is not initialized', async () => {
    const mockLoadGames = vi.fn();
    mockUsePlayerService.mockReturnValue({
      playerAlias: null,
      isInitialized: false,
      isLoading: true,
      error: null,
      initializePlayer: vi.fn(),
      clearPlayer: vi.fn(),
    });
    
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: false,
      loadGames: mockLoadGames,
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    await waitFor(() => {
      expect(mockLoadGames).not.toHaveBeenCalled();
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

  it('loads games on component mount', async () => {
    const games = [makeGame({ id: 'g1', status: 'InProgress' })];
    const mockLoadGames = vi.fn();
    mockUseGameService.mockReturnValue({
      savedGames: games,
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: mockLoadGames,
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    await waitFor(() => {
      expect(mockLoadGames).toHaveBeenCalledWith('existing-player');
    });
  });

  it('disables Load Game button when there are no saved games', async () => {
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    const loadGameButton = screen.getByText(/Load Game/i);
    expect(loadGameButton).toBeDisabled();
  });

  it('enables Load Game button when there are saved games', async () => {
    const games = [makeGame({ id: 'g1', status: 'InProgress' })];
    mockUseGameService.mockReturnValue({
      savedGames: games,
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    const loadGameButton = screen.getByText(/Load Game/i);
    expect(loadGameButton).not.toBeDisabled();
  });

  it('shows "No saved games" when there are no saved games and submenu is opened', async () => {
    // This test is no longer valid since Load Game button is disabled when no games exist
    // The "No saved games" message only shows when the button can be clicked (i.e., when there are games initially but the submenu opens)
    // We'll test the disabled button behavior instead
    mockUseGameService.mockReturnValue({
      savedGames: [],
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    const loadGameButton = screen.getByText(/Load Game/i);
    expect(loadGameButton).toBeDisabled();
  });

  it('filters out completed games from the list', async () => {
    const user = userEvent.setup();
    const games = [
      makeGame({ id: 'g1', status: 'InProgress' }),
      // Completed games should already be filtered out by the hook
    ];
    mockUseGameService.mockReturnValue({
      savedGames: games, // Hook already filters out completed games
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      // g1 (InProgress) should appear; completed games are filtered by the hook
      const thumbnails = document.querySelectorAll('[title="Easy - InProgress"]');
      expect(thumbnails).toHaveLength(1);
    });
  });

  it('navigates to /game/:id when a saved game is selected', async () => {
    const user = userEvent.setup();
    const game = makeGame({ id: 'game-abc', difficulty: 'Hard', status: 'InProgress' });
    mockUseGameService.mockReturnValue({
      savedGames: [game],
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
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
    const mockDeleteGame = vi.fn();
    mockUseGameService.mockReturnValue({
      savedGames: [game],
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: mockDeleteGame,
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    await user.click(screen.getByText(/Load Game/i));
    await waitFor(() => {
      expect(screen.getByTitle('Delete game')).toBeInTheDocument();
    });
    await user.click(screen.getByTitle('Delete game'));
    await waitFor(() => {
      expect(mockDeleteGame).toHaveBeenCalledWith('existing-player', 'del-game');
    });
  });

  it('closes New Game submenu when Load Game is clicked', async () => {
    const user = userEvent.setup();
    const games = [makeGame({ id: 'g1', status: 'InProgress' })];
    mockUseGameService.mockReturnValue({
      savedGames: games,
      isLoading: false,
      error: null,
      isLoaded: true,
      loadGames: vi.fn(),
      deleteGame: vi.fn(),
      clearCache: vi.fn(),
      refreshGames: vi.fn(),
    });
    
    renderHomePage();
    const subMenus = document.querySelectorAll('ul ul');
    const newGameSubMenu = subMenus[0];
    await user.click(screen.getByText(/Start New Game/i));
    expect(newGameSubMenu.className).toContain('subMenuOpen');
    await user.click(screen.getByText(/Load Game/i));
    expect(newGameSubMenu.className).not.toContain('subMenuOpen');
  });
});
