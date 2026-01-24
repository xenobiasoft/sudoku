using DepenMock.XUnit;
using Sudoku.Application.DTOs;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Application.DTOs;

public class GameDtoTests : BaseTestByType<GameDto>
{
    [Fact]
    public void FromGame_WithValidGame_CreatesCorrectDto()
    {
        // Arrange
        var game = CreateTestGame();

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        dto.Id.Should().Be(game.Id.Value.ToString());
        dto.PlayerAlias.Should().Be(game.PlayerAlias.Value);
        dto.Difficulty.Should().Be(game.Difficulty.Name);
        dto.Status.Should().Be(game.Status.ToString());
        dto.CreatedAt.Should().Be(game.CreatedAt);
        dto.StartedAt.Should().Be(game.StartedAt);
        dto.CompletedAt.Should().Be(game.CompletedAt);
        dto.PausedAt.Should().Be(game.PausedAt);
        dto.Statistics.Should().NotBeNull();
        dto.Cells.Should().NotBeNull();
        dto.Cells.Count.Should().Be(game.GetCells().Count);
        dto.MoveHistory.Should().BeEquivalentTo(game.MoveHistory);
    }

    [Fact]
    public void FromGame_WithStartedGame_IncludesStartedAt()
    {
        // Arrange
        var game = CreateTestGame();
        game.StartGame();

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        dto.StartedAt.Should().NotBeNull();
        dto.StartedAt.Should().Be(game.StartedAt);
    }

    [Fact]
    public void FromGame_WithPausedGame_IncludesPausedAt()
    {
        // Arrange
        var game = CreateTestGame();
        game.StartGame();
        game.PauseGame();

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        dto.PausedAt.Should().NotBeNull();
        dto.PausedAt.Should().Be(game.PausedAt);
    }

    [Fact]
    public void FromGame_MapsAllCellsCorrectly()
    {
        // Arrange
        var game = CreateTestGame();
        var expectedCellCount = 81; // 9x9 grid

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        dto.Cells.Should().NotBeNull();
        dto.Cells.Count.Should().Be(expectedCellCount);
        
        for (int i = 0; i < expectedCellCount; i++)
        {
            var originalCell = game.GetCells()[i];
            var dtoCell = dto.Cells[i];
            
            dtoCell.Row.Should().Be(originalCell.Row);
            dtoCell.Column.Should().Be(originalCell.Column);
            dtoCell.Value.Should().Be(originalCell.Value);
            dtoCell.IsFixed.Should().Be(originalCell.IsFixed);
            dtoCell.HasValue.Should().Be(originalCell.HasValue);
        }
    }

    [Fact]
    public void FromGame_MapsMoveHistoryCorrectly()
    {
        // Arrange
        var game = CreateTestGame();
        game.StartGame();

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        foreach (var moveHistory in game.MoveHistory)
        {
            moveHistory.Should().BeEquivalentTo(dto.MoveHistory);
        }
    }

    [Fact]
    public void FromGame_MapsStatisticsCorrectly()
    {
        // Arrange
        var game = CreateTestGame();

        // Act
        var dto = GameDto.FromGame(game);

        // Assert
        dto.Statistics.Should().NotBeNull();
        dto.Statistics.TotalMoves.Should().Be(game.Statistics.TotalMoves);
        dto.Statistics.ValidMoves.Should().Be(game.Statistics.ValidMoves);
        dto.Statistics.InvalidMoves.Should().Be(game.Statistics.InvalidMoves);
        dto.Statistics.PlayDuration.Should().Be(game.Statistics.PlayDuration);
        dto.Statistics.AccuracyPercentage.Should().Be(game.Statistics.AccuracyPercentage);
    }

    [Fact]
    public void Constructor_WithAllParameters_CreatesCorrectDto()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var playerAlias = "TestPlayer";
        var difficulty = "Medium";
        var status = "InProgress";
        var statistics = new GameStatisticsDto(10, 8, 2, TimeSpan.FromMinutes(5), 80.0);
        var createdAt = DateTime.UtcNow.AddMinutes(-10);
        var startedAt = DateTime.UtcNow.AddMinutes(-5);
        var completedAt = (DateTime?)null;
        var pausedAt = (DateTime?)null;
        var cells = new List<CellDto>();
        var moveHistory = new List<MoveHistoryDto>();

        // Act
        var dto = new GameDto(id, playerAlias, difficulty, status, statistics, createdAt, startedAt, completedAt, pausedAt, cells, moveHistory);

        // Assert
        dto.Id.Should().Be(id);
        dto.PlayerAlias.Should().Be(playerAlias);
        dto.Difficulty.Should().Be(difficulty);
        dto.Status.Should().Be(status);
        dto.Statistics.Should().Be(statistics);
        dto.CreatedAt.Should().Be(createdAt);
        dto.StartedAt.Should().Be(startedAt);
        dto.CompletedAt.Should().Be(completedAt);
        dto.PausedAt.Should().Be(pausedAt);
        dto.Cells.Should().BeEquivalentTo(cells);
        dto.MoveHistory.Should().BeEquivalentTo(moveHistory);
    }

    private static SudokuGame CreateTestGame()
    {
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var cells = new List<Cell>();
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells.Add(Cell.CreateEmpty(i, j));
            }
        }

        return SudokuGame.Create(playerAlias, difficulty, cells);
    }
}