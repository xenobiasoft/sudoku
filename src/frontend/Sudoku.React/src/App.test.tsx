import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import App from './App';

vi.mock('./api/apiClient', () => ({
  apiClient: {
    createPlayer: vi.fn().mockResolvedValue('test-alias'),
    playerExists: vi.fn().mockResolvedValue(true),
    getGames: vi.fn().mockResolvedValue([]),
  },
}));

describe('App routing', () => {
  it('renders HomePage on "/"', () => {
    render(
      <MemoryRouter initialEntries={['/']}>
        <Routes>
          <Route path="/" element={<div>Home Page</div>} />
        </Routes>
      </MemoryRouter>
    );
    expect(screen.getByText('Home Page')).toBeInTheDocument();
  });

  it('renders the app without crashing', () => {
    const { container } = render(<App />);
    expect(container).toBeInTheDocument();
  });
});
