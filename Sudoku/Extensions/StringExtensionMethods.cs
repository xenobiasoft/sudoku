namespace XenobiaSoft.Sudoku.Extensions;

public static class StringExtensionMethods
{
	public static string Randomize(this string source)
	{
		var charArray = source.ToCharArray();

		for (var i = 0; i < source.Length; i++)
		{
			var j = (source.Length - i + 1) * RandomGenerator.RandomNumber(1, 10) % source.Length;

			(charArray[i], charArray[j]) = (charArray[j], charArray[i]);
		}
		var randomized = new string(charArray);

		return randomized;
	}
}