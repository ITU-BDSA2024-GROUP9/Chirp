using Chirp.Core.Classes;

namespace Chirp.CLI;

// THIS IS NOT AN INTERFACE, change to an interface
internal class UserInterface
{
    public static void ShowCheep(Cheep c)
    {
        var convertedTimestamp = DateTimeOffset.FromUnixTimeSeconds(c.Timestamp).ToString("MM/dd/yy HH:mm:ss");
        Console.WriteLine(c.Author + " @ " + convertedTimestamp + ": " + c.Message);
    }
}