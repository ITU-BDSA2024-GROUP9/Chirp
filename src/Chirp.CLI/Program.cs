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
                await Read(args);
            }
            else if (args[0] == "cheep")
            {
              //  await Cheep(args);
                await Task.Delay(1000);
                await Read(["", "1"]); // get the newest cheep
            }
        }

        async private static Task Read(string[] args)
        {
            try
                {
                    var csv = DatabaseClientService<Cheep>.Instance;
                    var list = await csv.Read(args.Length < 2 ? null : int.Parse(args[1]));
                    UserInterface.ShowCheeps(list);
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
        }

        async private static Task Cheep(string[] args)
        {
            try
                {
                    Console.WriteLine("cheeping");
                    var str = args[1];
                    var userName = Environment.UserName;
                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var cheep = new Cheep(userName, str, unixTimestamp);
                    var csv = DatabaseClientService<Cheep>.Instance;
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
// hi hi