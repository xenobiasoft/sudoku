using XenobiaSoft.Sudoku.Abstractions;

namespace XenobiaSoft.Sudoku.Services;

public class PlayerService : IPlayerService
{
    private static readonly string[] Adjectives = ["Swift", "Clever", "Brave", "Witty", "Silent", "Happy"];
    private static readonly string[] Animals = ["Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther"];

    public Task<string> CreateNewAsync()
    {
        var random = new Random();
        var adjective = Adjectives[random.Next(Adjectives.Length)];
        var animal = Animals[random.Next(Animals.Length)];
        var number = random.Next(10, 100); // Add randomness to avoid collisions

        return Task.FromResult($"{adjective}{animal}{number}");
    }
}