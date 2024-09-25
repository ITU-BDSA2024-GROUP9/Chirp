using Chirp.Core.Classes;

namespace Chirp.CLI;

// THIS IS NOT AN INTERFACE, change to an interface
internal class UserInterface
{
    public static void ShowCheep(Cheep c)
    {
        var convertedTimestamp = DateTimeOffset.FromUnixTimeSeconds(c.timestamp).ToString("MM/dd/yy HH:mm:ss");
        Console.WriteLine(c.author + " @ " + convertedTimestamp + ": " + c.message);
    }

    public static void ShowCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps)
        {
            ShowCheep(cheep);
        }
    }
}