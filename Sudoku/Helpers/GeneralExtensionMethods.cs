using Newtonsoft.Json;

namespace XenobiaSoft.Sudoku.Helpers;

public static class GeneralExtensionMethods
{
	public static bool IsEmpty(this string[,] source)
	{
		var isEmpty = true;

		for (var col = 0; col < source.GetLength(0); col++)
		{
			for (var row = 0; row < source.GetLength(1); row++)
			{
				isEmpty = isEmpty && string.IsNullOrWhiteSpace(source[row, col]);
			}
		}

		return isEmpty;
	}

	public static bool IsEmpty(this int[,] source)
	{
		var isEmpty = true;

		for (var col = 0; col < source.GetLength(0); col++)
		{
			for (var row = 0; row < source.GetLength(1); row++)
			{
				isEmpty = isEmpty && source[row, col] == 0;
			}
		}

		return isEmpty;
	}

	public static string ToJson(this object obj, bool formatted = false)
	{
		var formatting = formatted ? Formatting.Indented : Formatting.None;
		return JsonConvert.SerializeObject(obj, formatting);
	}
}