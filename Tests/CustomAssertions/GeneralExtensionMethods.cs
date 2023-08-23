namespace UnitTests.CustomAssertions;

public static class GeneralExtensionMethods
{
    public static IntArrayAssertions Should(this int[,] instance)
    {
        return new IntArrayAssertions(instance);
    }

    public static StringArrayAssertions Should(this string[,] instance)
    {
        return new StringArrayAssertions(instance);
    }

    public static bool IsEquivalentTo(this string[,] instance, string[,] compareTo)
    {
        return instance.Rank == compareTo.Rank &&
               Enumerable
	               .Range(0, instance.Rank)
	               .All(x => instance.GetLength(x) == compareTo.GetLength(x) && instance.Cast<string>().SequenceEqual(compareTo.Cast<string>()));
	}

    public static bool IsEquivalentTo(this int[,] instance, int[,] compareTo)
	{
		return instance.Rank == compareTo.Rank &&
		       Enumerable
			       .Range(0, instance.Rank)
			       .All(x => instance.GetLength(x) == compareTo.GetLength(x) && instance.Cast<int>().SequenceEqual(compareTo.Cast<int>()));
	}
}