using Sudoku.Infrastructure.Services.Solvers;

namespace UnitTests.Infrastructure.Services;

public class BitwiseSolverEngineTests
{
    // Canonical Wikipedia example puzzle (unique solution).
    private const string UniquePuzzle =
        "530070000600195000098000060800060003400803001700020006060000280000419005000080079";

    [Fact]
    public void Solve_KnownUniquePuzzle_FillsEveryCellWithAValidCompleteGrid()
    {
        var grid = ToGrid(UniquePuzzle);

        var solved = BitwiseSolverEngine.Solve(grid);

        solved.Should().BeTrue();
        grid.Should().OnlyContain(v => v >= 1 && v <= 9);
        IsCompleteValidGrid(grid).Should().BeTrue();
    }

    [Fact]
    public void Solve_GivenClues_KeepsThoseCluesInTheSolution()
    {
        var clues = ToGrid(UniquePuzzle);
        var grid = (int[])clues.Clone();

        BitwiseSolverEngine.Solve(grid);

        for (var i = 0; i < 81; i++)
        {
            if (clues[i] != 0)
            {
                grid[i].Should().Be(clues[i]);
            }
        }
    }

    [Fact]
    public void Solve_InconsistentGrid_ReturnsFalse()
    {
        var grid = new int[81];
        grid[0] = 5;
        grid[1] = 5; // duplicate in the same row/box

        BitwiseSolverEngine.Solve(grid).Should().BeFalse();
    }

    [Fact]
    public void CountSolutions_UniquePuzzle_ReturnsOne()
    {
        var grid = ToGrid(UniquePuzzle);

        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(1);
    }

    [Fact]
    public void CountSolutions_EmptyGrid_StopsAtCap()
    {
        var grid = new int[81]; // wide-open, astronomically many solutions

        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(2);
    }

    [Fact]
    public void CountSolutions_UnderConstrainedPuzzle_ReturnsMoreThanOne()
    {
        // The unique puzzle with several clues removed admits multiple solutions.
        var grid = ToGrid(UniquePuzzle);
        grid[2] = 0;
        grid[3] = 0;
        grid[20] = 0;
        grid[40] = 0;
        grid[60] = 0;

        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().BeGreaterThan(1);
    }

    [Fact]
    public void CountSolutions_DoesNotMutateInputGrid()
    {
        var grid = ToGrid(UniquePuzzle);
        var snapshot = (int[])grid.Clone();

        BitwiseSolverEngine.CountSolutions(grid, cap: 2);

        grid.Should().Equal(snapshot);
    }

    private static int[] ToGrid(string board)
    {
        var grid = new int[81];
        for (var i = 0; i < 81; i++)
        {
            grid[i] = board[i] is >= '1' and <= '9' ? board[i] - '0' : 0;
        }
        return grid;
    }

    private static bool IsCompleteValidGrid(int[] grid)
    {
        for (var unit = 0; unit < 9; unit++)
        {
            var rowSeen = new HashSet<int>();
            var colSeen = new HashSet<int>();
            var boxSeen = new HashSet<int>();

            for (var k = 0; k < 9; k++)
            {
                if (!rowSeen.Add(grid[unit * 9 + k]))
                {
                    return false;
                }

                if (!colSeen.Add(grid[k * 9 + unit]))
                {
                    return false;
                }

                var boxRow = (unit / 3) * 3 + k / 3;
                var boxCol = (unit % 3) * 3 + k % 3;
                if (!boxSeen.Add(grid[boxRow * 9 + boxCol]))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
