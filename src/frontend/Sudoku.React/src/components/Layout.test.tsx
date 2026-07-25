import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Layout from './Layout';

function renderLayout(children: React.ReactNode, initialPath = '/') {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Layout>{children}</Layout>
    </MemoryRouter>
  );
}

describe('Layout', () => {
  it('renders the serif wordmark on the home route', () => {
    renderLayout(<div>content</div>, '/');
    expect(screen.getByText('Sudoku')).toBeInTheDocument();
  });

  it('renders a theme toggle button', () => {
    renderLayout(<div>content</div>, '/');
    expect(screen.getByRole('button', { name: /switch to (dark|light) theme/i })).toBeInTheDocument();
  });

  it('renders a back affordance on inner routes', () => {
    renderLayout(<div>content</div>, '/profile');
    expect(screen.getByRole('button', { name: /home/i })).toBeInTheDocument();
  });

  it('renders children inside main', () => {
    renderLayout(<span>test child</span>, '/');
    expect(screen.getByText('test child')).toBeInTheDocument();
  });

  it('hides the header when hideHeader is set', () => {
    render(
      <MemoryRouter>
        <Layout hideHeader>
          <span>only child</span>
        </Layout>
      </MemoryRouter>
    );
    expect(screen.queryByRole('button', { name: /switch to (dark|light) theme/i })).not.toBeInTheDocument();
    expect(screen.getByText('only child')).toBeInTheDocument();
  });

  it('widens the content column when wide is set', () => {
    const { container } = render(
      <MemoryRouter>
        <Layout wide>
          <span>wide child</span>
        </Layout>
      </MemoryRouter>
    );
    // CSS Modules hash class names, so match on the stable prefix.
    const column = container.querySelector('[class*="column"]');
    expect(column?.className).toMatch(/columnWide/);
  });

  it('does not widen the content column by default', () => {
    const { container } = render(
      <MemoryRouter>
        <Layout>
          <span>narrow child</span>
        </Layout>
      </MemoryRouter>
    );
    const column = container.querySelector('[class*="column"]');
    expect(column?.className).not.toMatch(/columnWide/);
  });
});
