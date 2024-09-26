using Chirp.Core.Classes;
using Chirp.Core.Interfaces;

namespace Chirp.Razor.Services
{
    public class MockCheepService : ICheepService
    {
        // These would normally be loaded from a database for example
        private List<CheepViewModel> _cheeps;

        public List<CheepViewModel> GetCheeps()
        {
            
            _cheeps = new List<CheepViewModel>();

            for (int i = 0; i < 971; i++)
            {
                _cheeps.Add(new CheepViewModel("test", "test", UnixTimeStampToDateTimeString(123131231)));
            }

            return _cheeps;
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
