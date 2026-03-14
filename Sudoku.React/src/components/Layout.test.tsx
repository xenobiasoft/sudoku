import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import Layout from './Layout';

describe('Layout', () => {
  it('renders the header logo', () => {
    render(<Layout><div>content</div></Layout>);
    const logo = screen.getByAltText('XenobiaSoft Sudoku');
    expect(logo).toBeInTheDocument();
  });

  it('renders children inside main', () => {
    render(<Layout><span>test child</span></Layout>);
    expect(screen.getByText('test child')).toBeInTheDocument();
  });
});
