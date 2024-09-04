using Chirp.CLI.Classes;

namespace Chirp.CLI;
    internal class UserInterface
    {
        public static void ShowCheep(Cheep c)
        {
            var convertedTimestamp = DateTimeOffset.FromUnixTimeSeconds(c.Timestamp).ToString("MM/dd/yy HH:mm:ss");
            Console.WriteLine(c.Author + " @ " + convertedTimestamp + ": " + c.Message);
        }
    }