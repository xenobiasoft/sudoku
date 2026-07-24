using DepenMock.Attributes;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

[LogOutput(LogOutputTiming.Always)]
public class BoardSizeTests : MoqBaseTestByType<BoardSize>
{
    [Theory]
    [MemberData(nameof(AllDifficulties))]
    public void Supports_ForNine_AllowsEveryDifficulty(GameDifficulty difficulty)
    {
        // Act / Assert
        BoardSize.Nine.Supports(difficulty).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SixteenSupportedDifficulties))]
    public void Supports_ForSixteen_AllowsEasyAndMedium(GameDifficulty difficulty)
    {
        // Act / Assert
        BoardSize.Sixteen.Supports(difficulty).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SixteenUnsupportedDifficulties))]
    public void Supports_ForSixteen_RejectsHardAndExpert(GameDifficulty difficulty)
    {
        // Act / Assert
        BoardSize.Sixteen.Supports(difficulty).Should().BeFalse();
    }

    [Fact]
    public void SupportedDifficulties_ForNine_ContainsAllFourDifficulties()
    {
        // Act / Assert
        BoardSize.Nine.SupportedDifficulties.Should().HaveCount(4);
    }

    [Fact]
    public void SupportedDifficulties_ForSixteen_ContainsOnlyEasyAndMedium()
    {
        // Act / Assert
        BoardSize.Sixteen.SupportedDifficulties.Should().BeEquivalentTo([GameDifficulty.Easy, GameDifficulty.Medium]);
    }

    public static TheoryData<GameDifficulty> AllDifficulties =>
        [GameDifficulty.Easy, GameDifficulty.Medium, GameDifficulty.Hard, GameDifficulty.Expert];

    public static TheoryData<GameDifficulty> SixteenSupportedDifficulties =>
        [GameDifficulty.Easy, GameDifficulty.Medium];

    public static TheoryData<GameDifficulty> SixteenUnsupportedDifficulties =>
        [GameDifficulty.Hard, GameDifficulty.Expert];
}
