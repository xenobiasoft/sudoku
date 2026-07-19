using Newtonsoft.Json;
using Sudoku.Infrastructure.Models;

namespace UnitTests.Infrastructure.Models;

/// <summary>
/// Guards the zero-migration back-compat guarantee (FR-10): every pre-existing Cosmos/blob
/// document was written before the <c>gridSize</c> property existed, so it must still
/// deserialize as a 9x9 document. These fixtures are literal JSON strings shaped like the
/// documents actually written pre-change — not round-tripped through the current serializer —
/// so they fail if the <c>GridSize</c> initializer default (9) is ever removed or renamed.
/// </summary>
public class GridSizeBackCompatTests
{
    [Fact]
    public void SudokuGameDocument_WhenGridSizeAbsentFromJson_DeserializesAsNine()
    {
        // Arrange
        const string json = """
            {
              "id": "11111111-1111-1111-1111-111111111111",
              "gameId": "11111111-1111-1111-1111-111111111111",
              "profileId": "22222222-2222-2222-2222-222222222222",
              "displayName": "Player One",
              "difficulty": "Easy",
              "status": 1,
              "cells": [],
              "statistics": {
                "totalMoves": 0,
                "validMoves": 0,
                "invalidMoves": 0,
                "hintsUsed": 0,
                "playDuration": "00:00:00",
                "lastMoveAt": null
              },
              "moveHistory": [],
              "createdAt": "2025-01-01T00:00:00Z",
              "startedAt": null,
              "completedAt": null,
              "pausedAt": null
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<SudokuGameDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(9);
    }

    [Fact]
    public void SudokuGameDocument_WhenGridSizeSixteenPresentInJson_DeserializesAsSixteen()
    {
        // Arrange
        const string json = """
            {
              "id": "11111111-1111-1111-1111-111111111111",
              "gameId": "11111111-1111-1111-1111-111111111111",
              "profileId": "22222222-2222-2222-2222-222222222222",
              "displayName": "Player One",
              "difficulty": "Easy",
              "gridSize": 16,
              "status": 1,
              "cells": [],
              "statistics": {
                "totalMoves": 0,
                "validMoves": 0,
                "invalidMoves": 0,
                "hintsUsed": 0,
                "playDuration": "00:00:00",
                "lastMoveAt": null
              },
              "moveHistory": [],
              "createdAt": "2025-01-01T00:00:00Z",
              "startedAt": null,
              "completedAt": null,
              "pausedAt": null
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<SudokuGameDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(16);
    }

    [Fact]
    public void SudokuPuzzleDocument_WhenGridSizeAbsentFromJson_DeserializesAsNine()
    {
        // Arrange
        const string json = """
            {
              "PuzzleId": "puzzle-1",
              "Difficulty": "easy",
              "Cells": []
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<SudokuPuzzleDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(9);
    }

    [Fact]
    public void SudokuPuzzleDocument_WhenGridSizeSixteenPresentInJson_DeserializesAsSixteen()
    {
        // Arrange
        const string json = """
            {
              "PuzzleId": "puzzle-1",
              "Difficulty": "easy",
              "GridSize": 16,
              "Cells": []
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<SudokuPuzzleDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(16);
    }

    [Fact]
    public void GameCompletionDocument_WhenGridSizeAbsentFromJson_DeserializesAsNine()
    {
        // Arrange
        const string json = """
            {
              "id": "11111111-1111-1111-1111-111111111111",
              "gameId": "11111111-1111-1111-1111-111111111111",
              "profileId": "22222222-2222-2222-2222-222222222222",
              "difficulty": "Easy",
              "playDuration": "00:12:00",
              "completedAt": "2025-01-01T00:00:00Z"
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<GameCompletionDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(9);
    }

    [Fact]
    public void GameCompletionDocument_WhenGridSizeSixteenPresentInJson_DeserializesAsSixteen()
    {
        // Arrange
        const string json = """
            {
              "id": "11111111-1111-1111-1111-111111111111",
              "gameId": "11111111-1111-1111-1111-111111111111",
              "profileId": "22222222-2222-2222-2222-222222222222",
              "difficulty": "Easy",
              "gridSize": 16,
              "playDuration": "00:12:00",
              "completedAt": "2025-01-01T00:00:00Z"
            }
            """;

        // Act
        var document = JsonConvert.DeserializeObject<GameCompletionDocument>(json);

        // Assert
        document.Should().NotBeNull();
        document!.GridSize.Should().Be(16);
    }
}
