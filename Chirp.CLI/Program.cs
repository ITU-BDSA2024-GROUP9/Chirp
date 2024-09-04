using Chirp.Core.Classes;

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
                    using StreamReader reader = new("./chirp_cli_db.csv");
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        i++;
                        string[]? text = (await reader.ReadLineAsync())?.Split(",\"");
                        if (text == null || i == 1)
                        {
                            continue;
                        }
                        string[]? text2 = text[1].Split("\",");
                        Cheep cheep = new Cheep(text[0],text2[0],long.Parse(text2[1]));
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
