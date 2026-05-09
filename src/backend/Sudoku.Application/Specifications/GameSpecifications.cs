using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Specifications;

public class GameByProfileIdSpecification : BaseSpecification<SudokuGame>
{
    public GameByProfileIdSpecification(ProfileId profileId)
    {
        AddCriteria(game => game.ProfileId == profileId);
        AddOrderByDescending(game => game.CreatedAt);
    }
}

public class GameByProfileIdAndStatusSpecification : BaseSpecification<SudokuGame>
{
    public GameByProfileIdAndStatusSpecification(ProfileId profileId, GameStatusEnum statusEnum)
    {
        AddCriteria(game => game.ProfileId == profileId && game.Status == statusEnum);
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

public class GameByProfileIdAndDifficultySpecification : BaseSpecification<SudokuGame>
{
    public GameByProfileIdAndDifficultySpecification(ProfileId profileId, GameDifficulty difficulty)
    {
        AddCriteria(game => game.ProfileId == profileId && game.Difficulty == difficulty);
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
    public CompletedGamesSpecification(ProfileId? profileId = null)
    {
        if (profileId != null)
        {
            AddCriteria(game => game.Status == GameStatusEnum.Completed && game.ProfileId == profileId);
        }
        else
        {
            AddCriteria(game => game.Status == GameStatusEnum.Completed);
        }
        AddOrderByDescending(game => game.CompletedAt);
    }
}
