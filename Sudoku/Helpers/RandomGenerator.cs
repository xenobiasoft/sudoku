namespace XenobiaSoft.Sudoku.Helpers;

public static class RandomGenerator
{
	public static int RandomNumber(int? min = null, int? max = null)
	{
		var rnd = new Random((int)DateTime.Now.Ticks);

		if (max.HasValue)
		{
			return min.HasValue ? rnd.Next(min.Value, max.Value) : rnd.Next(max.Value);
		}

		return rnd.Next();
	}
}