import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import NewGamePage from './NewGamePage';
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
    createGame: vi.fn(),
  },
}));

import { apiClient } from '../api/apiClient';

function renderNewGamePage(difficulty: string) {
  return render(
    <MemoryRouter initialEntries={[`/new/${difficulty}`]}>
      <Routes>
        <Route path="/new/:difficulty" element={<NewGamePage />} />
      </Routes>
    </MemoryRouter>
  );
}

beforeEach(() => {
  mockNavigate.mockClear();
  vi.mocked(apiClient.createGame).mockClear();
  localStorage.setItem('playerAlias', 'test-player');
});

describe('NewGamePage', () => {
  it('shows a loading message with the difficulty', () => {
    vi.mocked(apiClient.createGame).mockResolvedValue(makeGame());
    renderNewGamePage('Easy');
    expect(screen.getByText(/Creating Easy puzzle/i)).toBeInTheDocument();
  });

  it('navigates to game page after successfully creating a game', async () => {
    const game = makeGame({ id: 'new-game-id' });
    vi.mocked(apiClient.createGame).mockResolvedValue(game);
    renderNewGamePage('Medium');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/game/new-game-id', { replace: true });
    });
  });

  it('navigates home when alias is missing', async () => {
    localStorage.removeItem('playerAlias');
    vi.mocked(apiClient.createGame).mockResolvedValue(makeGame());
    renderNewGamePage('Hard');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });

  it('navigates home when createGame fails', async () => {
    vi.mocked(apiClient.createGame).mockRejectedValue(new Error('Network error'));
    renderNewGamePage('Easy');
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });

  it('shows the numeric loader with digits 1-9', () => {
    vi.mocked(apiClient.createGame).mockResolvedValue(makeGame());
    renderNewGamePage('Hard');
    for (let n = 1; n <= 9; n++) {
      expect(screen.getByText(n.toString())).toBeInTheDocument();
    }
  });
});
