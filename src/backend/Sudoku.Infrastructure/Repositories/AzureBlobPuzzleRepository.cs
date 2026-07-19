using Microsoft.Extensions.Logging;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Services;

namespace Sudoku.Infrastructure.Repositories;

public class AzureBlobPuzzleRepository(
    IAzureStorageService storageService,
    IPuzzleGenerator puzzleGenerator,
    ILogger<AzureBlobPuzzleRepository> logger) : IPuzzleRepository, IPuzzleBlobStorage
{
    private const string Container = "sudoku-puzzles";

    public async Task<SudokuPuzzle> CreateAsync(GameDifficulty difficulty, BoardSize size)
    {
        var puzzle = await puzzleGenerator.GeneratePuzzleAsync(difficulty, size);
        var document = MapToDocument(puzzle, size);
        var blobName = $"{size.Size}x{size.Size}/{difficulty.Name.ToLowerInvariant()}/{puzzle.PuzzleId}.json";
        await storageService.SaveAsync(Container, blobName, document);
        logger.LogDebug("Stored puzzle {PuzzleId} for {Difficulty} ({Size})", puzzle.PuzzleId, difficulty.Name, size);
        return puzzle;
    }

    Task<SudokuPuzzle> IPuzzleRepository.CreateAsync(string alias, GameDifficulty difficulty) =>
        throw new NotSupportedException();

    public async Task DeleteAsync(string prefix, string puzzleId)
    {
        var blobName = $"{prefix}/{puzzleId}.json";
        await storageService.DeleteAsync(Container, blobName);
    }

    public async IAsyncEnumerable<string> GetPuzzleIdsAsync(string prefix)
    {
        await foreach (var blobName in storageService.GetBlobNamesAsync(Container, $"{prefix}/"))
        {
            yield return Path.GetFileNameWithoutExtension(blobName);
        }
    }

    public async Task<SudokuPuzzle?> LoadAsync(string prefix, string puzzleId)
    {
        var blobName = $"{prefix}/{puzzleId}.json";
        var document = await storageService.LoadAsync<SudokuPuzzleDocument>(Container, blobName);
        return document is null ? null : MapToPuzzle(document);
    }

    Task<SudokuPuzzle> IPuzzleRepository.LoadAsync(string alias, string puzzleId) =>
        throw new NotSupportedException();

    Task<IEnumerable<SudokuPuzzle>> IPuzzleRepository.LoadAllAsync(string alias) =>
        throw new NotSupportedException();

    public Task<SudokuPuzzle> ResetAsync(string alias, string puzzleId) =>
        throw new NotSupportedException();

    public Task SaveAsync(SudokuPuzzle gameState) =>
        throw new NotSupportedException();

    public Task<SudokuPuzzle> UndoAsync(string alias, string puzzleId) =>
        throw new NotSupportedException();

    private static SudokuPuzzleDocument MapToDocument(SudokuPuzzle puzzle, BoardSize size) =>
        new()
        {
            PuzzleId = puzzle.PuzzleId,
            Difficulty = puzzle.Difficulty.Name.ToLowerInvariant(),
            GridSize = size.Size,
            Cells = puzzle.Cells.Select(c => new CellDocument
            {
                Row = c.Row,
                Column = c.Column,
                Value = c.Value,
                IsFixed = c.IsFixed,
                PossibleValues = c.PossibleValues
            }).ToList()
        };

    private static SudokuPuzzle MapToPuzzle(SudokuPuzzleDocument document)
    {
        var difficulty = GameDifficulty.FromName(document.Difficulty);
        var size = BoardSize.FromValue(document.GridSize);
        var cells = document.Cells.Select(c => Cell.Create(c.Row, c.Column, size, c.Value, c.IsFixed));
        return SudokuPuzzle.Create(document.PuzzleId, difficulty, size, cells);
    }
}
