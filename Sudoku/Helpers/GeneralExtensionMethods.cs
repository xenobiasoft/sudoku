// ReSharper disable once CheckNamespace
namespace XenobiaSoft.Sudoku;

public static class GeneralExtensionMethods
{
	public static bool IsEmpty(this string[,] source)
	{
		var isEmpty = true;

		for (var col = 0; col < source.GetLength(0); col++)
		{
			for (var row = 0; row < source.GetLength(1); row++)
			{
				isEmpty = isEmpty && string.IsNullOrWhiteSpace(source[col, row]);
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
				isEmpty = isEmpty && source[col, row] == 0;
			}
		}

		return isEmpty;
	}
}