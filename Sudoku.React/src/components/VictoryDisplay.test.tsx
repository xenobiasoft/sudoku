import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import VictoryDisplay from './VictoryDisplay';
import userEvent from '@testing-library/user-event';

describe('VictoryDisplay', () => {
  it('renders the victory title', () => {
    render(<VictoryDisplay onClose={() => {}} />);
    expect(screen.getByText(/Puzzle Solved/)).toBeInTheDocument();
  });

  it('renders the congratulations subtitle', () => {
    render(<VictoryDisplay onClose={() => {}} />);
    expect(screen.getByText(/Congratulations/)).toBeInTheDocument();
  });

  it('renders the Back to Home button', () => {
    render(<VictoryDisplay onClose={() => {}} />);
    expect(screen.getByRole('button', { name: /Back to Home/i })).toBeInTheDocument();
  });

  it('calls onClose when Back to Home button is clicked', async () => {
    const user = userEvent.setup();
    const onClose = vi.fn();
    render(<VictoryDisplay onClose={onClose} />);
    await user.click(screen.getByRole('button', { name: /Back to Home/i }));
    expect(onClose).toHaveBeenCalledOnce();
  });
});
