using Sudoku.Infrastructure.Mappers;
using Sudoku.Infrastructure.Models;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Mappers;

public class SudokuGameMapperTests : MoqBaseTestByType<SudokuGameDocument>
{
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
        restored.Statistics.HintsRemaining.Should().Be(game.Statistics.HintsRemaining);
    }
}
