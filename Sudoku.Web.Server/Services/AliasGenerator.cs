namespace Sudoku.Web.Server.Services;

public static class AliasGenerator
{
    private static readonly string[] Adjectives = ["Swift", "Clever", "Brave", "Witty", "Silent", "Happy"];
    private static readonly string[] Animals = ["Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther"];

    [Obsolete("Use PlayerService instead")]
    public static string GenerateAlias()
    {
        var random = new Random();
        var adjective = Adjectives[random.Next(Adjectives.Length)];
        var animal = Animals[random.Next(Animals.Length)];
        var number = random.Next(10, 100); // Add randomness to avoid collisions

        return $"{adjective}{animal}{number}";
    }
}
