namespace XenobiaSoft.Sudoku.Helpers;

public static class StringExtensionMethods
{
	public static string Randomize(this string source)
	{
		var charArray = source.ToCharArray();
		var rnd = new Random((int)DateTime.Now.Ticks);

		for (var i = 0; i < source.Length; i++)
		{
			var j = (source.Length - i + 1) * rnd.Next(1, 10) % source.Length;

			(charArray[i], charArray[j]) = (charArray[j], charArray[i]);
		}
		var randomized = new string(charArray);

		if (randomized == source)
		{
			source.Randomize();
		}

		return randomized;
	}
}