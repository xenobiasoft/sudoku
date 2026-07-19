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
  return render(
    <MemoryRouter initialEntries={['/select-difficulty']}>
      <SelectDifficultyPage />
    </MemoryRouter>
  );
}

beforeEach(() => {
  mockNavigate.mockClear();
  mockUsePlayerService.mockReturnValue({ isNewPlayer: false, isInitialized: true, playerAlias: 'test-user' });
});

describe('SelectDifficultyPage', () => {
  it('renders four difficulty cards', () => {
    renderPage();
    expect(screen.getByRole('button', { name: /Easy/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Medium/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Hard/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /Expert/i })).toBeInTheDocument();
  });

  it('navigates to /new/Easy?size=9 when Easy is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Easy/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Easy?size=9');
  });

  it('navigates to /new/Medium?size=9 when Medium is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Medium/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Medium?size=9');
  });

  it('navigates to /new/Hard?size=9 when Hard is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Hard/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Hard?size=9');
  });

  it('navigates to /new/Expert?size=9 when Expert is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Expert/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Expert?size=9');
  });

  it('defaults to the Classic 9x9 size toggle option', () => {
    renderPage();
    expect(screen.getByRole('button', { name: /Classic 9×9/i })).toHaveAttribute('aria-pressed', 'true');
    expect(screen.getByRole('button', { name: /Giant 16×16/i })).toHaveAttribute('aria-pressed', 'false');
  });

  it('navigates with ?size=16 when Giant 16x16 is selected before picking a difficulty', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /Giant 16×16/i }));
    await user.click(screen.getByRole('button', { name: /Expert/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/new/Expert?size=16');
  });

  it('navigates to / when the header back affordance is clicked', async () => {
    const user = userEvent.setup();
    renderPage();
    await user.click(screen.getByRole('button', { name: /home/i }));
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('redirects to / when new player visits', () => {
    mockUsePlayerService.mockReturnValue({ isNewPlayer: true, isInitialized: false, playerAlias: null });
    renderPage();
    expect(mockNavigate).toHaveBeenCalledWith('/');
  });
});
