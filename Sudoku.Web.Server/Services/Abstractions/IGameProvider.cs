using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameProvider
{
    GameModel Game { get; }
}