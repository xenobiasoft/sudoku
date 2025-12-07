using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Specifications;

public class GameByPlayerSpecification : BaseSpecification<SudokuGame>
{
    public GameByPlayerSpecification(PlayerAlias playerAlias)
    {
        AddCriteria(game => game.PlayerAlias == playerAlias);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class GameByPlayerAndStatusSpecification : BaseSpecification<SudokuGame>
{
    public GameByPlayerAndStatusSpecification(PlayerAlias playerAlias, GameStatusEnum statusEnum)
    {
        AddCriteria(game => game.PlayerAlias == playerAlias && game.Status == statusEnum);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class GameByStatusSpecification : BaseSpecification<SudokuGame>
{
    public GameByStatusSpecification(GameStatusEnum statusEnum)
    {
        AddCriteria(game => game.Status == statusEnum);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class GameByDifficultySpecification : BaseSpecification<SudokuGame>
{
    public GameByDifficultySpecification(GameDifficulty difficulty)
    {
        AddCriteria(game => game.Difficulty == difficulty);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class GameByPlayerAndDifficultySpecification : BaseSpecification<SudokuGame>
{
    public GameByPlayerAndDifficultySpecification(PlayerAlias playerAlias, GameDifficulty difficulty)
    {
        AddCriteria(game => game.PlayerAlias == playerAlias && game.Difficulty == difficulty);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class RecentGamesSpecification : BaseSpecification<SudokuGame>
{
    public RecentGamesSpecification(int count = 10)
    {
        AddOrderByDescending(game => game.CreatedAt);
        ApplyPaging(0, count);
    }
}

public class CompletedGamesSpecification : BaseSpecification<SudokuGame>
{
    public CompletedGamesSpecification(PlayerAlias? playerAlias = null)
    {
        if (playerAlias != null)
        {
            AddCriteria(game => game.Status == GameStatusEnum.Completed && game.PlayerAlias == playerAlias);
        }
        else
        {
            AddCriteria(game => game.Status == GameStatusEnum.Completed);
        }
        AddOrderByDescending(game => game.CompletedAt);
    }
}