﻿using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public interface ISudokuPuzzle
{
    Cell FindCellWithFewestPossibleValues();
    Cell[] GetAllCells();
    Cell GetCell(int row, int col);
    IEnumerable<Cell> GetColumnCells(int col);
    IEnumerable<Cell> GetRowCells(int row);
    IEnumerable<Cell> GetMiniGridCells(int row, int col);
    bool IsSolved();
    bool IsValid();
    void Load(GameStateMemory gameState);
    void PopulatePossibleValues();
    void Restore(Cell[] cells);
    void SetCell(int row, int column, int? value);
    void SetCellWithFewestPossibleValues();
    IEnumerable<Cell> Validate();

    string PuzzleId { get; set; }
}