using Chirp.Core.Classes;
using SimpleDB.Services;

namespace Chirp.CLI
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            await Run(args);
        }

        async private static Task Run(string[] args)
        {
            if (args[0] == "read")
            {
                try
                {
                    var csv = CSVDatabaseService<Cheep>.Instance;
                    var list = await csv.Read(args[1] == "all" ? null : int.Parse(args[1]));
                    foreach (Cheep cheep in list)
                    {
                        UserInterface.ShowCheep(cheep);
                    }

                }
                catch (IOException e)
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
                    var csv = CSVDatabaseService<Cheep>.Instance;
                    await csv.Store(cheep);
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
// hi