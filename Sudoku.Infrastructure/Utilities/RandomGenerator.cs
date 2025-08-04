namespace Sudoku.Infrastructure.Utilities;

public static class RandomGenerator
{
	public static int RandomNumber(int? min = null, int? max = null)
    {
        var rnd = Random.Shared;

		if (max.HasValue)
		{
			return min.HasValue ? rnd.Next(min.Value, max.Value) : rnd.Next(max.Value);
		}

		return rnd.Next();
	}
}