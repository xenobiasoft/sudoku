using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

public interface IGameProvider
{
    GameModel Game { get; }
}