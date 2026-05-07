using Sudoku.Domain.Entities;
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

    public static SudokuGame CreateGameWithCells(IEnumerable<Cell> cells)
    {
        return CreateGame(cells);
    }

    public static SudokuGame CreateGameWithDifficulty(GameDifficulty difficulty)
    {
        var game = CreateGame(CellsFactory.CreateIncompleteCells(), difficulty: difficulty);

        game.StartGame();

        return game;
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

    private static SudokuGame CreateGame(IEnumerable<Cell> cells, ProfileId? profileId = null, PlayerAlias? displayName = null, GameDifficulty? difficulty = null)
    {
        var withDifficulty = difficulty ?? GameDifficulty.Easy;
        var withProfileId = profileId ?? ProfileId.New();
        var withDisplayName = displayName ?? PlayerAlias.Create("DefaultPlayer");
        var game = SudokuGame.Create(
            withProfileId,
            withDisplayName,
            withDifficulty,
            cells);

        return game;
    }
}
