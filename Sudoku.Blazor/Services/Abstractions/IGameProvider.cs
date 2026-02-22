using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.Abstractions;

public interface IGameProvider
{
    GameModel Game { get; }
}