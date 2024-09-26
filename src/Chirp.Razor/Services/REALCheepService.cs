using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using System.ComponentModel.Design;
using Chirp.Core.Helpers;

namespace Chirp.Razor.Services
{
    public class REALCheepService : ICheepService
    {
        // These would normally be loaded from a database for example
        private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

        public List<CheepViewModel> GetCheeps()
        {
            var database = new DBFacade();
            return database.Query(@"
               SELECT
                    user.username,
                    text,
                    pub_date
                FROM
                    message
                JOIN user ON message.author_id = user.user_id
            ");
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author)
        {
            // filter by the provided author name
            return _cheeps.Where(x => x.Author == author).ToList();
        }

        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
