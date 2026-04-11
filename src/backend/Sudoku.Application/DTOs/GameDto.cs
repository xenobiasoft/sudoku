using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.DTOs;

public record GameDto(
    string Id,
    string PlayerAlias,
    string Difficulty,
    string Status,
    GameStatisticsDto Statistics,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? PausedAt,
    List<CellDto> Cells,
    List<MoveHistoryDto> MoveHistory)
{
    public static GameDto FromGame(SudokuGame game)
    {
        return new GameDto(
            game.Id.Value.ToString(),
            game.PlayerAlias.Value,
            game.Difficulty.Name,
            game.Status.ToString(),
            GameStatisticsDto.FromGameStatistics(game.Statistics),
            game.CreatedAt,
            game.StartedAt,
            game.CompletedAt,
            game.PausedAt,
            game.GetCells().Select(CellDto.FromCell).ToList(),
            game.MoveHistory.Select(MoveHistoryDto.FromMoveHistory).ToList()
        );
    }
}

public record GameStatisticsDto(
    int TotalMoves,
    int ValidMoves,
    int InvalidMoves,
    TimeSpan PlayDuration,
    double AccuracyPercentage)
{
    public static GameStatisticsDto FromGameStatistics(GameStatistics statistics)
    {
        return new GameStatisticsDto(
            statistics.TotalMoves,
            statistics.ValidMoves,
            statistics.InvalidMoves,
            statistics.PlayDuration,
            statistics.AccuracyPercentage
        );
    }
}

public record CellDto(
    int Row,
    int Column,
    int? Value,
    bool IsFixed,
    bool HasValue,
    List<int> PossibleValues)
{
    public static CellDto FromCell(Cell cell)
    {
        return new CellDto(
            cell.Row,
            cell.Column,
            cell.Value,
            cell.IsFixed,
            cell.HasValue,
            cell.PossibleValues.ToList()
        );
    }
}

public record MoveHistoryDto(
    int Row,
    int Column,
    int? PreviousValue,
    int? NewValue)
{
    public static MoveHistoryDto FromMoveHistory(MoveHistory moveHistory)
    {
        return new MoveHistoryDto(
            moveHistory.Row,
            moveHistory.Column,
            moveHistory.PreviousValue,
            moveHistory.NewValue
        );
    }
}