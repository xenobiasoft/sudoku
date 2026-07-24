using Sudoku.Domain.ValueObjects;

namespace Sudoku.Domain.Exceptions;

public class UnsupportedDifficultyException(BoardSize size, GameDifficulty difficulty)
    : DomainException($"{difficulty.Name} is not available for {size} boards.");
