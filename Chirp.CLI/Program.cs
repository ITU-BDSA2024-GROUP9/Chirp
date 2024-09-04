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

        private static void Run(string[] args)
        {
            if (args[0] == "read")
            {
                try
                {
                    CSVDatabaseService<Cheep> csv = new();
                    IEnumerable<Cheep> list = csv.Read(1);
                    Console.WriteLine(list.Count());
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
                    CSVDatabaseService<Cheep> csv = new();
                    csv.Store(cheep);
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
