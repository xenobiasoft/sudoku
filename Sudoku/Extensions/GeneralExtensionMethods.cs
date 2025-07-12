namespace XenobiaSoft.Sudoku.Extensions;

public static class ListExtensionMethods
{
    public static IList<int> Randomize(this IList<int> list)
    {
        if (list == null || list.Count <= 1) return list;

        var rng = new Random();
        var randomizedList = list.ToList();

        for (var i = randomizedList.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (randomizedList[i], randomizedList[j]) = (randomizedList[j], randomizedList[i]);
        }

        return randomizedList;
    }
}
