import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import SelectDifficultyPage from './SelectDifficultyPage';

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual<typeof import('react-router-dom')>('react-router-dom');
  return { ...actual, useNavigate: () => mockNavigate };
});

const mockUsePlayerService = vi.fn();
vi.mock('../hooks/usePlayerService', () => ({
  usePlayerService: () => mockUsePlayerService(),
}));

function renderPage() {
  return render(<MemoryRouter><SelectDifficultyPage /></MemoryRouter>);
}

beforeEach(() => {
  mockNavigate.mockClear();
  mockUsePlayerService.mockReturnValue({ isNewPlayer: false, isInitialized: true, playerAlias: 'test-user' });
});

describe('SelectDifficultyPage', () => {
  it('renders three difficulty buttons', () => {
    renderPage();
    expect(screen.getByRole('button', { name: 'Easy' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Medium' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Hard' })).toBeInTheDocument();
  });

  it('navigates to /new/Easy when Easy is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: 'Easy' }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Easy');
  });

  it('navigates to /new/Medium when Medium is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: 'Medium' }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Medium');
  });

  it('navigates to /new/Hard when Hard is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: 'Hard' }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Hard');
  });

  it('navigates to / when Back is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Back/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('redirects to / when new player visits', () => {
    mockUsePlayerService.mockReturnValue({ isNewPlayer: true, isInitialized: false, playerAlias: null });
    renderPage();
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });
});
