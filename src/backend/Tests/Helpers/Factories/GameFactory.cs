using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Factories;

public static class GameFactory
{
    public static SudokuGame CreateCompletedGame()
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells());

        game.StartGame();
        game.MakeMove(0, 0, 5);
        game.MakeMove(1, 1, 7);

        return game;
    }

    public static SudokuGame CreateGameForPlayer(ProfileId profileId, PlayerAlias displayName)
    {
        return CreateGame(CellsFactory.CreateIncompleteCells(), profileId: profileId, displayName: displayName);
    }

    public static SudokuGame CreateEmptyGame()
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells());

        return game;
    }

    public static SudokuGame CreateGameInProgress()
    {
        return CreateStartedGame();
    }

    public static SudokuGame CreateGameNotStarted()
    {
        return CreateGame(CellsFactory.CreateEmptyCells());
    }

    public static SudokuGame CreatePausedGame()
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells());
        game.StartGame();
        game.PauseGame();
        return game;
    }

    public static SudokuGame CreateGameWithPossibleValue(int row, int column, int value)
    {
        var game = CreateGame(CellsFactory.CreateEmptyCells());
        game.StartGame();
        game.AddPossibleValue(row, column, value);
        return game;
    }

    public static SudokuGame CreateGameWithCells(IEnumerable<Cell> cells)
    {
        return CreateGame(cells);
    }

    public static SudokuGame CreateGameWithDifficulty(GameDifficulty difficulty, BoardSize? size = null)
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells(), difficulty: difficulty, size: size);

        game.StartGame();

        return game;
    }

    /// <summary>
    /// Rebuilds a game the way a repository does, bypassing the <see cref="SudokuGame.Create"/>
    /// invariants. Use this for size/difficulty combinations that are no longer offered but may
    /// still exist in storage from before they were retired (e.g. 16x16 Hard/Expert).
    /// </summary>
    public static SudokuGame CreateLegacyGame(GameDifficulty difficulty, BoardSize size)
    {
        return SudokuGame.Reconstitute(
            GameId.New(),
            ProfileId.New(),
            PlayerAlias.Create("DefaultPlayer"),
            difficulty,
            size,
            GameStatusEnum.InProgress,
            GameStatistics.Create(),
            CellsFactory.CreateIncompleteCells(),
            [],
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            null);
    }

    public static SudokuGame CreateInvalidGame()
    {
        var game = CreateGame(CellsFactory.CreateInvalidCells());

        game.StartGame();

        return game;
    }

    public static SudokuGame CreateStartedGame()
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells());

        game.StartGame();

        return game;
    }

    private static SudokuGame CreateGame(IEnumerable<Cell> cells, ProfileId? profileId = null, PlayerAlias? displayName = null, GameDifficulty? difficulty = null, BoardSize? size = null)
    {
        var withDifficulty = difficulty ?? GameDifficulty.Easy;
        var withProfileId = profileId ?? ProfileId.New();
        var withDisplayName = displayName ?? PlayerAlias.Create("DefaultPlayer");
        var withSize = size ?? BoardSize.Nine;
        var game = SudokuGame.Create(
            withProfileId,
            withDisplayName,
            withDifficulty,
            withSize,
            cells);

        return game;
    }
}
