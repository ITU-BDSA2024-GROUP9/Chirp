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
                    using StreamWriter writer = File.AppendText("./chirp_cli_db.csv");
                    var cheep = args[1];
                    var userName = Environment.UserName;
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    await writer.WriteLineAsync("\n" + userName + ',' + '"' + cheep + '"' + ',' + unixTimestamp);
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be written to:");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
