using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using System.ComponentModel.Design;
using Chirp.Core.Helpers;

namespace Chirp.Razor.Services
{
    public class CheepService : ICheepService
    {
        private readonly DBFacade _database;

        public CheepService()
        {
            _database = new DBFacade();
        }
        public List<CheepViewModel> GetCheeps()
        {
            _database.EnsureConnectionInitialized();
            try
            {
                return _database.Query(@"
               SELECT
                    user.username,
                    text,
                    pub_date
                FROM
                    message
                JOIN user ON message.author_id = user.user_id
            ");
            }
            finally
            {
                _database.Dispose();
            }
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author)
        {
            _database.EnsureConnectionInitialized();
            try
            { 
                return _database.Query(@"
               SELECT
                    user.username,
                    text,
                    pub_date
                FROM
                    message
                JOIN user ON message.author_id = user.user_id
                WHERE user.username = @author
            ", new Dictionary<string, object> { { "@author", author } });
            }
            finally
            {
                _database.Dispose();
            }
        }
    }
}