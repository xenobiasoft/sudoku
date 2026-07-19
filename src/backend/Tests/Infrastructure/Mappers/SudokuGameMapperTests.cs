using DepenMock.Attributes;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Mappers;
using Sudoku.Infrastructure.Models;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Mappers;

[LogOutput(LogOutputTiming.Always)]
public class SudokuGameMapperTests : MoqBaseTestByType<SudokuGameDocument>
{
    [Theory]
    [InlineData("Easy")]
    [InlineData("Medium")]
    [InlineData("Hard")]
    [InlineData("Expert")]
    public void RoundTrip_PreservesDifficulty(string difficultyName)
    {
        // Arrange
        var difficulty = GameDifficulty.FromName(difficultyName);
        var game = GameFactory.CreateGameWithDifficulty(difficulty);

        // Act
        var document = SudokuGameMapper.ToDocument(game);
        var restored = SudokuGameMapper.ToDomain(document);

        // Assert
        restored.Difficulty.Should().Be(difficulty);
    }

    [Fact]
    public void RoundTrip_PreservesHintCellsAndHintsUsed()
    {
        // Arrange - reveal a hint so the game has a hint cell and a non-zero HintsUsed
        var game = GameFactory.CreateGameWithCells(CellsFactory.CreateEmptyCells());
        game.StartGame();
        var (row, column, _) = game.RevealHint(PuzzleFactory.GetSolvedPuzzle());

        // Act
        var document = SudokuGameMapper.ToDocument(game);
        var restored = SudokuGameMapper.ToDomain(document);

        // Assert
        restored.GetCell(row, column).IsHint.Should().BeTrue();
        restored.Statistics.HintsUsed.Should().Be(1);
        restored.Statistics.HintsRemainingFor(restored.Size).Should().Be(game.Statistics.HintsRemainingFor(game.Size));
    }

    [Fact]
    public void ToDomain_WhenDocumentGridSizePropertyNeverSet_DefaultsToNine()
    {
        // Arrange — GridSize is left at its property initializer, matching a document written
        // before FR-10 introduced the property (Newtonsoft keeps the initializer when absent).
        var document = CreateDocumentForSize(BoardSize.Nine);

        // Act
        var restored = SudokuGameMapper.ToDomain(document);

        // Assert
        restored.Size.Should().Be(BoardSize.Nine);
    }

    [Fact]
    public void ToDomain_WhenDocumentGridSizeIsSixteen_ReconstitutesAsSixteenBySixteen()
    {
        // Arrange
        var document = CreateDocumentForSize(BoardSize.Sixteen);

        // Act
        var restored = SudokuGameMapper.ToDomain(document);

        // Assert
        restored.Size.Should().Be(BoardSize.Sixteen);
        restored.GetCells().Should().HaveCount(BoardSize.Sixteen.CellCount);
    }

    [Fact]
    public void ToDomain_WhenGridSizeDisagreesWithCellCount_Throws()
    {
        // Arrange — GridSize claims 16x16 but the cell collection is still 9x9-sized (81 cells)
        var game = GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy);
        var document = SudokuGameMapper.ToDocument(game);
        document.GridSize = 16;

        // Act
        var act = () => SudokuGameMapper.ToDomain(document);

        // Assert
        act.Should().Throw<InvalidPuzzleException>();
    }

    private static SudokuGameDocument CreateDocumentForSize(BoardSize size)
    {
        var cells = new List<CellDocument>();
        for (var row = 0; row < size.Size; row++)
        {
            for (var column = 0; column < size.Size; column++)
            {
                cells.Add(new CellDocument { Row = row, Column = column, Value = null, IsFixed = false });
            }
        }

        return new SudokuGameDocument
        {
            Id = Guid.NewGuid().ToString(),
            GameId = Guid.NewGuid().ToString(),
            ProfileId = Guid.NewGuid().ToString(),
            DisplayName = "Player",
            Difficulty = GameDifficulty.Easy.Name,
            GridSize = size.Size,
            Cells = cells
        };
    }
}
