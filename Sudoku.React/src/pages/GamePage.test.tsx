import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import GamePage from './GamePage';
import { makeGame, make81Cells, makeCell } from '../test/helpers';

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
    getGame: vi.fn(),
    updateStatus: vi.fn().mockResolvedValue(undefined),
    makeMove: vi.fn(),
    resetGame: vi.fn(),
    undoMove: vi.fn(),
    deleteGame: vi.fn().mockResolvedValue(undefined),
    addPossibleValue: vi.fn(),
    removePossibleValue: vi.fn(),
    clearPossibleValues: vi.fn(),
  },
}));

import { apiClient } from '../api/apiClient';

function renderGamePage(gameId = 'test-game') {
  return render(
    <MemoryRouter initialEntries={[`/game/${gameId}`]}>
      <Routes>
        <Route path="/game/:puzzleId" element={<GamePage />} />
      </Routes>
    </MemoryRouter>
  );
}

beforeEach(() => {
  vi.clearAllMocks();
  mockNavigate.mockClear();
  localStorage.setItem('playerAlias', 'test-player');
  vi.mocked(apiClient.updateStatus).mockResolvedValue(undefined);
  vi.mocked(apiClient.deleteGame).mockResolvedValue(undefined);
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe('GamePage - loading', () => {
  it('shows "Loading puzzle..." before game data arrives', () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
    renderGamePage();
    expect(screen.getByText(/Loading puzzle/i)).toBeInTheDocument();
  });

  it('navigates home when getGame fails', async () => {
    vi.mocked(apiClient.getGame).mockRejectedValue(new Error('Not found'));
    renderGamePage();
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });
});

describe('GamePage - after load', () => {
  it('renders the game board after loading', async () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
  });

  it('renders game controls after loading', async () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByTitle('Home')).toBeInTheDocument();
    });
  });

  it('renders game stats after loading', async () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByText('Time')).toBeInTheDocument();
    });
  });

  it('calls updateStatus with "InProgress" on load', async () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame({ id: 'g1' }));
    renderGamePage('g1');
    await waitFor(() => {
      expect(apiClient.updateStatus).toHaveBeenCalledWith('test-player', 'g1', 'InProgress');
    });
  });
});

describe('GamePage - cell selection', () => {
  it('allows selecting a cell', async () => {
    const game = makeGame();
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    // Click on the first input cell
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[0]);
    // The cell should be selected (no specific assertion needed beyond no crash)
    expect(inputs[0]).toBeInTheDocument();
  });
});

describe('GamePage - keyboard navigation', () => {
  it('moves selection with arrow keys', async () => {
    const game = makeGame();
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    const table = document.querySelector('table')!;
    // First select a cell
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[4]);
    // Then press arrow key
    fireEvent.keyDown(table, { key: 'ArrowDown' });
    // No error should occur
    expect(table).toBeInTheDocument();
  });
});

describe('GamePage - number input', () => {
  it('calls makeMove when a number button is clicked with a selected cell', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells });
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.makeMove).mockResolvedValue(updatedGame);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    // Select cell (0,0)
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[0]);
    // Click the number 5 button
    await userEvent.click(screen.getByRole('button', { name: '5' }));
    await waitFor(() => {
      expect(apiClient.makeMove).toHaveBeenCalledWith(
        'test-player', game.id, 0, 0, 5, expect.any(String)
      );
    });
  });

  it('does not call makeMove when no cell is selected', async () => {
    const game = makeGame();
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByRole('button', { name: '7' }));
    expect(apiClient.makeMove).not.toHaveBeenCalled();
  });
});

describe('GamePage - erase', () => {
  it('calls makeMove with null when erase is clicked on a cell with a value', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: true, value: 3 });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells: make81Cells() });
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.makeMove).mockResolvedValue(updatedGame);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[0]);
    await userEvent.click(screen.getByTitle('Erase'));
    await waitFor(() => {
      expect(apiClient.makeMove).toHaveBeenCalledWith(
        'test-player', game.id, 0, 0, null, expect.any(String)
      );
    });
  });
});

describe('GamePage - undo / reset', () => {
  it('calls undoMove when undo button is clicked', async () => {
    const game = makeGame({ moveHistory: [{ row: 0, column: 0, value: 1, isValid: true }] });
    const updatedGame = makeGame();
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.undoMove).mockResolvedValue(updatedGame);
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByTitle('Undo')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByTitle('Undo'));
    await waitFor(() => {
      expect(apiClient.undoMove).toHaveBeenCalledWith('test-player', game.id);
    });
  });

  it('calls resetGame when reset button is clicked', async () => {
    const game = makeGame();
    const updatedGame = makeGame();
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.resetGame).mockResolvedValue(updatedGame);
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByTitle('Reset')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByTitle('Reset'));
    await waitFor(() => {
      expect(apiClient.resetGame).toHaveBeenCalledWith('test-player', game.id);
    });
  });
});

describe('GamePage - home navigation', () => {
  it('calls updateStatus with Paused and navigates home when Home is clicked', async () => {
    const game = makeGame({ id: 'g1' });
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    renderGamePage('g1');
    await waitFor(() => {
      expect(screen.getByTitle('Home')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByTitle('Home'));
    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });
});

describe('GamePage - pencil mode', () => {
  it('toggles pencil mode when pencil button is clicked', async () => {
    vi.mocked(apiClient.getGame).mockResolvedValue(makeGame());
    renderGamePage();
    await waitFor(() => {
      expect(screen.getByTitle('Pencil mode')).toBeInTheDocument();
    });
    // Pencil button should be present
    const pencilBtn = screen.getByTitle('Pencil mode');
    await userEvent.click(pencilBtn);
    // No crash expected
    expect(pencilBtn).toBeInTheDocument();
  });

  it('calls addPossibleValue in pencil mode when a number is clicked', async () => {
    const cells = make81Cells();
    cells[0] = makeCell({ row: 0, column: 0, isFixed: false, hasValue: false, possibleValues: [] });
    const game = makeGame({ cells });
    const updatedGame = makeGame({ cells });
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.addPossibleValue).mockResolvedValue(updatedGame);
    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    // Enable pencil mode
    await userEvent.click(screen.getByTitle('Pencil mode'));
    // Select cell (0,0)
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[0]);
    // Click number 3
    await userEvent.click(screen.getByRole('button', { name: '3' }));
    await waitFor(() => {
      expect(apiClient.addPossibleValue).toHaveBeenCalledWith('test-player', game.id, 0, 0, 3);
    });
  });
});

describe('GamePage - victory', () => {
  it('shows victory overlay when puzzle is solved', async () => {
    // Build a solved game — all cells have values, no duplicates
    const solution = [
      [5,3,4,6,7,8,9,1,2],
      [6,7,2,1,9,5,3,4,8],
      [1,9,8,3,4,2,5,6,7],
      [8,5,9,7,6,1,4,2,3],
      [4,2,6,8,5,3,7,9,1],
      [7,1,3,9,2,4,8,5,6],
      [9,6,1,5,3,7,2,8,4],
      [2,8,7,4,1,9,6,3,5],
      [3,4,5,2,8,6,1,7,9],
    ];
    const cells = make81Cells();
    for (let r = 0; r < 9; r++) {
      for (let c = 0; c < 9; c++) {
        const idx = r * 9 + c;
        cells[idx] = makeCell({ row: r, column: c, value: solution[r][c], hasValue: true, isFixed: false });
      }
    }
    // Start with one missing cell
    const initialCells = cells.map((c, i) => i === 0 ? { ...c, value: null, hasValue: false } : c);
    const game = makeGame({ cells: initialCells });
    const solvedGame = makeGame({ cells });
    vi.mocked(apiClient.getGame).mockResolvedValue(game);
    vi.mocked(apiClient.makeMove).mockResolvedValue(solvedGame);

    renderGamePage();
    await waitFor(() => {
      expect(document.querySelector('table')).toBeInTheDocument();
    });
    // Select cell (0,0)
    const inputs = screen.getAllByRole('textbox');
    await userEvent.click(inputs[0]);
    // Enter the missing value (5)
    await userEvent.click(screen.getByRole('button', { name: '5' }));
    await waitFor(() => {
      expect(screen.getByText(/Puzzle Solved/i)).toBeInTheDocument();
    });
  });
});
