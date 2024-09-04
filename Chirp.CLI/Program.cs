using Chirp.Core.Classes;
using SimpleDB.Services;

namespace Chirp.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        private static async void Run(string[] args)
        {
            if (args[0] == "read")
            {
                try
                {
                    Console.WriteLine("readning");
                    CSVDatabaseService<Cheep> csv = new();
                    IAsyncEnumerable<Cheep> list = await csv.ReadAsync(1);
                    await foreach(Cheep c in list)
                        UserInterface.ShowCheep(c);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else if (args[0] == "cheep")
            {
                try
                {
                    Console.WriteLine("cheeping");
                    var str = args[1];
                    var userName = Environment.UserName;
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var cheep = new Cheep(userName, str, unixTimestamp);
                    CSVDatabaseService<Cheep> csv = new();
                    await csv.StoreAsync(cheep);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be written to:");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
