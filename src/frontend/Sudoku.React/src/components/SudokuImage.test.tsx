import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import SudokuImage from './SudokuImage';

describe('SudokuImage', () => {
  it('renders the sudoku image', () => {
    render(<SudokuImage />);
    const img = screen.getByAltText('Sudoku');
    expect(img).toBeInTheDocument();
    expect(img).toHaveAttribute('src', '/images/sudoku_image.jpg');
  });
});
