using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Events;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;
using InvalidMoveException = Sudoku.Domain.Exceptions.InvalidMoveException;

namespace UnitTests.Domain;

public class SudokuGameTests : BaseTestByType<SudokuGame>
{
    [Fact]
    public void AbandonGame_WhenGameIsCompleted_ThrowsGameAlreadyCompletedException()
    {
        // Arrange
        var sut = GameFactory.CreateCompletedGame();

        // Act
        Action act = () => sut.AbandonGame();

        // Assert
        act.Should().Throw<GameAlreadyCompletedException>();
    }

    [Fact]
    public void AbandonGame_WhenGameIsNotCompleted_ChangesStatusToAbandoned()
    {
        // Arrange
        var sut = GameFactory.CreateEmptyGame();
        sut.StartGame();

        // Act
        sut.AbandonGame();

        // Assert
        sut.Status.Should().Be(GameStatusEnum.Abandoned);
        sut.DomainEvents.Should().Contain(e => e is GameAbandonedEvent);
    }

    [Fact]
    public void Create_WithValidParameters_CreatesGameWithCorrectProperties()
    {
        // Arrange
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        var cells = GenerateEmptyCells();

        // Act
        var sut = SudokuGame.Create(playerAlias, difficulty, cells);

        // Assert
        sut.Id.Should().NotBeNull();
        sut.PlayerAlias.Should().Be(playerAlias);
        sut.Difficulty.Should().Be(difficulty);
        sut.Status.Should().Be(GameStatusEnum.NotStarted);
        sut.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sut.GetCells().Count.Should().Be(81);
        sut.DomainEvents.Should().ContainSingle(e => e is GameCreatedEvent);
    }

    [Fact]
    public void IsGameComplete_WithAllCellsFilledCorrectly_ReturnsTrue()
    {
        // Arrange
        var sut = GameFactory.CreateCompletedGame();

        // Act
        var result = sut.IsGameComplete();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsGameComplete_WithNotAllCellsFilled_ReturnsFalse()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        var result = sut.IsGameComplete();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void MakeMove_ClearEmptyCell_DoesNotThrow()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        Action act = () => sut.MakeMove(0, 0, null);

        // Assert
        act.Should().NotThrow();
        sut.GetCell(0, 0).Value.Should().BeNull();
    }

    [Fact]
    public void MakeMove_ClearFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var sut = CreateGameWithCells(new[]
        {
            Cell.CreateFixed(0, 0, 5)
        });
        sut.StartGame();

        // Act
        Action act = () => sut.MakeMove(0, 0, null);

        // Assert
        act.Should().Throw<CellIsFixedException>();
    }

    [Fact]
    public void MakeMove_OnFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var sut = CreateGameWithCells(new[]
        {
            Cell.CreateFixed(0, 0, 5)
        });
        sut.StartGame();

        // Act
        Action act = () => sut.MakeMove(0, 0, 6);

        // Assert
        act.Should().Throw<CellIsFixedException>();
    }

    [Fact]
    public void MakeMove_WhenGameIsNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var sut = GameFactory.CreateCompletedGame();

        // Act
        Action act = () => sut.MakeMove(0, 0, 5);

        // Assert
        act.Should().Throw<GameNotInProgressException>();
    }

    [Fact]
    public void MakeMove_WithInvalidMove_ThrowsInvalidMoveException()
    {
        // Arrange
        var sut = CreateGameWithCells(new[]
        {
            Cell.Create(0, 0, 5, false),
            Cell.Create(0, 1, null, false)
        });
        sut.StartGame();

        // Act
        Action act = () => sut.MakeMove(0, 1, 5);

        // Assert
        act.Should().Throw<InvalidMoveException>();
    }

    [Fact]
    public void MakeMove_WithNullValue_ClearsCellValue()
    {
        // Arrange
        var sut = CreateGameWithCells(new[]
        {
            Cell.Create(0, 0, 5, false)
        });
        sut.StartGame();

        // Act
        sut.MakeMove(0, 0, null);

        // Assert
        sut.GetCell(0, 0).Value.Should().BeNull();
        sut.GetCell(0, 0).HasValue.Should().BeFalse();
    }

    [Fact]
    public void MakeMove_WithNullValue_RaisesDomainEventWithNullValue()
    {
        // Arrange
        var sut = CreateGameWithCells(new[]
        {
            Cell.Create(0, 0, 5, false)
        });
        sut.StartGame();
        var initialEventCount = sut.DomainEvents.Count;

        // Act
        sut.MakeMove(0, 0, null);

        // Assert
        var moveEvent = sut.DomainEvents.OfType<MoveMadeEvent>().Last();
        moveEvent.Value.Should().BeNull();
        moveEvent.Row.Should().Be(0);
        moveEvent.Column.Should().Be(0);
        sut.DomainEvents.Count.Should().Be(initialEventCount + 1);
    }

    [Fact]
    public void MakeMove_WithValidMove_RaisesDomainEvent()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();
        var initialEventCount = sut.DomainEvents.Count;

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.DomainEvents.Should().Contain(e => e is MoveMadeEvent);
        sut.DomainEvents.Count.Should().Be(initialEventCount + 1);
    }

    [Fact]
    public void MakeMove_WithValidMove_UpdatesCell()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.GetCell(0, 0).Value.Should().Be(5);
    }

    [Fact]
    public void PauseGame_WhenGameIsInProgress_ChangesStatusToPaused()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        sut.PauseGame();

        // Assert
        sut.Status.Should().Be(GameStatusEnum.Paused);
        sut.PausedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sut.DomainEvents.Should().Contain(e => e is GamePausedEvent);
    }

    [Fact]
    public void PauseGame_WhenGameIsNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var sut = GameFactory.CreateEmptyGame();

        // Act
        Action act = () => sut.PauseGame();

        // Assert
        act.Should().Throw<GameNotInProgressException>();
    }

    [Fact]
    public void ResumeGame_WhenGameIsNotPaused_ThrowsGameNotPausedException()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        Action act = () => sut.ResumeGame();

        // Assert
        act.Should().Throw<GameNotPausedException>();
    }

    [Fact]
    public void ResumeGame_WhenGameIsPaused_ChangesStatusToInProgress()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();
        sut.PauseGame();

        // Act
        sut.ResumeGame();

        // Assert
        sut.Status.Should().Be(GameStatusEnum.InProgress);
        sut.PausedAt.Should().BeNull();
        sut.DomainEvents.Should().Contain(e => e is GameResumedEvent);
    }

    [Fact]
    public void StartGame_WhenGameIsAlreadyInProgress_ThrowsGameNotInStartStateException()
    {
        // Arrange
        var sut = GameFactory.CreateGameInProgress();

        // Act
        Action act = () => sut.StartGame();

        // Assert
        act.Should().Throw<GameNotInStartStateException>();
    }

    [Fact]
    public void StartGame_WhenGameIsNotStarted_ChangesStatusToInProgress()
    {
        // Arrange
        var sut = GameFactory.CreateGameNotStarted();

        // Act
        sut.StartGame();

        // Assert
        sut.Status.Should().Be(GameStatusEnum.InProgress);
        sut.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sut.DomainEvents.Should().Contain(e => e is GameStartedEvent);
    }

    [Fact]
    public void UpdatePlayDuration_UpdatesStatisticsDuration()
    {
        // Arrange
        var sut = GameFactory.CreateCompletedGame();
        var duration = TimeSpan.FromMinutes(10);

        // Act
        sut.UpdatePlayDuration(duration);

        // Assert
        sut.Statistics.PlayDuration.Should().Be(duration);
    }

    private SudokuGame CreateGameWithCells(IEnumerable<Cell> specificCells)
    {
        var playerAlias = PlayerAlias.Create("TestPlayer");
        var difficulty = GameDifficulty.Medium;
        
        var cells = GenerateEmptyCells().ToList();
        
        foreach (var cell in specificCells)
        {
            var index = cells.FindIndex(c => c.Row == cell.Row && c.Column == cell.Column);
            if (index >= 0)
            {
                cells[index] = cell;
            }
        }
        
        return SudokuGame.Create(playerAlias, difficulty, cells);
    }

    private IEnumerable<Cell> GenerateEmptyCells()
    {
        var cells = new List<Cell>();
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(Cell.CreateEmpty(row, col));
            }
        }
        return cells;
    }
}