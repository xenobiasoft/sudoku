using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.States;

namespace UnitTests.Helpers.Factories;

public class GameModelFactory
{
    private static readonly GameModelFactory Instance = null!;

    private string _id = Guid.NewGuid().ToString();
    private string _playerAlias = string.Empty;
    private GameDifficulty _difficulty = GameDifficulty.Easy;
    private string _gameStatus = GameStatus.NotStarted;
    private List<CellModel> _cells = GenerateCellModels(GameDifficulty.Easy, true);
    private DateTime _completedAt = DateTime.UtcNow;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _pausedAt = DateTime.UtcNow;
    private DateTime _startedAt = DateTime.UtcNow;
    private GameStatisticsModel _statistics = new();

    private GameModelFactory()
    { }

    internal static GameModelFactory CreateInstance() => Instance ?? new();

    public static GameModelFactory Build()
    {
        return CreateInstance();
    }

    public GameModelFactory WithId(string id)
    {
        _id = id;
        return this;
    }

    public GameModelFactory WithCells(List<CellModel> cells)
    {
        _cells = cells;
        return this;
    }

    public GameModelFactory WithCompletedAt(DateTime completedAt)
    {
        _completedAt = completedAt;
        return this;
    }

    public GameModelFactory WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public GameModelFactory WithDifficulty(GameDifficulty difficulty)
    {
        _difficulty = difficulty;
        _cells = GenerateCellModels(difficulty);
        return this;
    }

    public GameModelFactory WithPausedAt(DateTime pausedAt)
    {
        _pausedAt = pausedAt;
        return this;
    }

    public GameModelFactory WithPlayerAlias(string playerAlias)
    {
        _playerAlias = playerAlias;
        return this;
    }

    public GameModelFactory WithStartedAt(DateTime startedAt)
    {
        _startedAt = startedAt;
        return this;
    }

    public GameModelFactory WithStatistics(GameStatisticsModel statistics)
    {
        _statistics = statistics;
        return this;
    }

    public GameModelFactory WithStatus(string statusEnum)
    {
        _gameStatus = statusEnum;
        return this;
    }

    public GameModel Create() => new GameModel
    {
        Id = _id,
        Cells = _cells,
        CompletedAt = _completedAt,
        CreatedAt = _createdAt,
        Difficulty = _difficulty,
        PausedAt = _pausedAt,
        PlayerAlias = _playerAlias,
        StartedAt = _startedAt,
        Statistics = _statistics,
        Status = _gameStatus,
    };

    public static GameModel GetEmptyPuzzle() => Build()
        .WithCells(ConvertCells(DefaultPuzzle))
        .Create();

    public static GameModel GetInvalidPuzzle() => Build()
        .WithCells(ConvertCells(InvalidPuzzle, true))
        .Create();

    public static GameModel GetSolvedPuzzle() => Build()
        .WithCells(ConvertCells(SolvedPuzzle))
        .Create();

    private static List<CellModel> GenerateCellModels(GameDifficulty difficulty, bool rotateGrid = false)
    {
        var values = GetCellValues(difficulty);

        return ConvertCells(values, rotateGrid);
    }

    private static int?[,] GetCellValues(GameDifficulty difficulty)
    {
        return difficulty switch
        {
            _ when difficulty.Equals(GameDifficulty.Easy) => EasyPuzzle,
            _ when difficulty.Equals(GameDifficulty.Medium) => MediumPuzzle,
            _ when difficulty.Equals(GameDifficulty.Hard) => HardPuzzle,
            _ when difficulty.Equals(GameDifficulty.Expert) => ExtremePuzzle,
            _ => DefaultPuzzle
        };
    }

    private static List<CellModel> ConvertCells(int?[,] values, bool rotateGrid = false)
    {
        if (rotateGrid)
        {
            values = RotateGrid(values);
        }

        var cells = new CellModel[81];
        var index = 0;
        for (var col = 0; col < 9; col++)
        {
            for (var row = 0; row < 9; row++)
            {
                var cell = new CellModel
                {
                    Row = row,
                    Column = col,
                    Value = values[row, col],
                    PossibleValues = []
                };
                cells[index++] = cell;
            }
        }
        return cells.ToList();
    }

    private static int?[,] RotateGrid(int?[,] values)
    {
        var grid = new int?[9, 9];

        for (var col = 0; col < 9; col++)
        {
            for (var row = 0; row < 9; row++)
            {
                grid[col, row] = values[row, col];
            }
        }

        return grid;
    }

    private static readonly int?[,] EasyPuzzle = {
        { 5, 3, null, null, 7, null, null, null, null },
        { 6, null, null, 1, 9, 5, null, null, null },
        { null, 9, 8, null, null, null, null, 6, null },
        { 8, null, null, null, 6, null, null, null, 3 },
        { 4, null, null, 8, null, 3, null, null, 1 },
        { 7, null, null, null, 2, null, null, null, 6 },
        { null, 6, null, null, null, null, 2, 8, null },
        { null, null, null, 4, 1, 9, null, null, 5 },
        { null, null, null, null, 8, null, null, 7, 9 }
    };

    private static readonly int?[,] MediumPuzzle = {
        { 7, null, null, null, null, null, 5, null, null },
        { null, null, 3, null, null, 7, null, 2, 8 },
        { 4, null, 2, 5, 8, null, null, null, null },
        { 8, null, null, null, 7, null, 2, null, null },
        { null, null, null, 2, 1, 3, null, null, null },
        { null, null, 9, null, 6, null, null, null, 4 },
        { null, null, null, null, 3, 8, 4, null, 9 },
        { 3, 8, null, 7, null, null, 6, null, null },
        { null, null, 1, null, null, null, null, null, 2 }
    };

    private static readonly int?[,] HardPuzzle = {
        { 7, null, 8, null, null, null, null, 2, null },
        { null, null, 1, 4, 8, null, null, null, 3 },
        { null, null, null, null, 5, 7, 4, null, null },
        { null, 7, null, 2, null, null, null, null, 1 },
        { 3, null, null, null, 6, null, null, null, 8 },
        { 1, null, null, null, null, 5, null, 4, null },
        { null, null, 7, 5, 1, null, null, null, null },
        { 8, null, null, null, 2, 6, 7, null, null },
        { null, 1, null, null, null, null, 6, null, 2 }
    };

    private static readonly int?[,] ExtremePuzzle = {
        { null, null, null, null, null, 8, null, null, 9 },
        { null, 5, null, null, 9, null, null, null, null },
        { null, null, 9, null, null, 4, 8, null, null },
        { null, null, 2, 1, 4, null, null, null, 3 },
        { null, null, 6, null, null, null, 9, null, null },
        { 4, null, null, null, 6, 7, 1, null, null },
        { null, null, null, 9, null, null, 3, null, null },
        { null, null, null, null, 2, null, null, 7, null },
        { 8, null, null, 4, null, null, null, null, null }
    };

    private static readonly int?[,] DefaultPuzzle = {
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null },
        { null, null, null, null, null, null, null, null, null }
    };

    private static readonly int?[,] InvalidPuzzle =
    {
        { 5, 3, null, null, 7, null, null, null, null },
        { 6, null, null, 1, 9, 5, null, null, null },
        { null, 9, 8, null, null, null, null, 6, null },
        { 8, null, null, null, 6, null, null, 5, 3 }, // Invalid row
        { 4, null, null, 8, null, 3, null, null, 1 },
        { 7, null, null, null, 2, null, null, null, 6 },
        { null, 6, null, null, null, null, 2, 8, null },
        { null, null, null, 4, 1, 9, null, null, 5 },
        { null, null, null, null, 8, null, null, 7 ,9}
    };

    private static readonly int?[,] SolvedPuzzle =
    {
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
        { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
        { 2, 3, 1, 6, 7, 4, 8, 9, 5 },
        { 8, 7, 5, 9, 1, 2, 3, 6, 4 },
        { 6, 9, 4, 5, 3, 8, 2, 1, 7 },
        { 3, 1, 7, 2, 6, 5, 9, 4, 8 },
        { 5, 4, 2, 8, 9, 7, 6, 3, 1 },
        { 9, 6, 8, 3, 4, 1, 5, 7, 2 }
    };
}