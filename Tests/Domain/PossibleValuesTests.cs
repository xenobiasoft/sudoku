using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

/// <summary>
/// This is a separate class to verify that the business rule "possible values cannot be entered for locked cells" 
/// is correctly implemented. It can be run manually until the test infrastructure issues are resolved.
/// </summary>
public class PossibleValuesTests
{
    public static void VerifyBusinessRules()
    {
        // Test 1: Adding possible values to a locked cell throws an exception
        var lockedCell = Cell.CreateFixed(0, 0, 5);
        try
        {
            lockedCell.AddPossibleValue(3);
            Console.WriteLine("Test 1 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 1 PASSED: Cannot add possible values to locked cells");
        }

        // Test 2: Removing possible values from a locked cell throws an exception
        try
        {
            lockedCell.RemovePossibleValue(3);
            Console.WriteLine("Test 2 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 2 PASSED: Cannot remove possible values from locked cells");
        }

        // Test 3: Clearing possible values from a locked cell throws an exception
        try
        {
            lockedCell.ClearPossibleValues();
            Console.WriteLine("Test 3 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 3 PASSED: Cannot clear possible values from locked cells");
        }

        // Test 4: At the game level, adding possible values to a locked cell throws an exception
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Easy;
        var cells = new List<Cell>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (row == 0 && col == 0)
                {
                    cells.Add(Cell.CreateFixed(row, col, 5));
                }
                else
                {
                    cells.Add(Cell.CreateEmpty(row, col));
                }
            }
        }
        var game = SudokuGame.Create(playerAlias, difficulty, cells);
        game.StartGame();
        
        try
        {
            game.AddPossibleValue(0, 0, 3);
            Console.WriteLine("Test 4 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 4 PASSED: Cannot add possible values to locked cells at game level");
        }

        // Test 5: At the game level, removing possible values from a locked cell throws an exception
        try
        {
            game.RemovePossibleValue(0, 0, 3);
            Console.WriteLine("Test 5 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 5 PASSED: Cannot remove possible values from locked cells at game level");
        }

        // Test 6: At the game level, clearing possible values from a locked cell throws an exception
        try
        {
            game.ClearPossibleValues(0, 0);
            Console.WriteLine("Test 6 FAILED: Should have thrown CellIsFixedException");
        }
        catch (CellIsFixedException)
        {
            Console.WriteLine("Test 6 PASSED: Cannot clear possible values from locked cells at game level");
        }

        Console.WriteLine("All tests completed. If all tests passed, the business rule is correctly implemented.");
    }
}