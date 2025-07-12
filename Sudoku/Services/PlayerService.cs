using XenobiaSoft.Sudoku.Abstractions;

namespace XenobiaSoft.Sudoku.Services;

public class PlayerService : IPlayerService
{
    private static readonly string[] Adjectives = ["Swift", "Clever", "Brave", "Witty", "Silent", "Happy"];
    private static readonly string[] Animals = ["Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther"];
    private static readonly Random RandomInstance = new Random();

    public Task<string> CreateNewAsync()
    {
        var adjective = Adjectives[RandomInstance.Next(Adjectives.Length)];
        var animal = Animals[RandomInstance.Next(Animals.Length)];
        var number = RandomInstance.Next(10, 100); // Add randomness to avoid collisions

        return Task.FromResult($"{adjective}{animal}{number}");
    }
}