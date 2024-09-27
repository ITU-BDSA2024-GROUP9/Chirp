using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using System.ComponentModel.Design;
using Chirp.Core.Helpers;

namespace Chirp.Razor.Services
{
    public class CheepService : ICheepService
    {
        public List<CheepViewModel> GetCheeps()
        {
            var database = new DBFacade();
            try
            {
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
            finally
            {
                database.Dispose();
            }
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author)
        {
            var database = new DBFacade();
            try
            { 
                return database.Query(@"
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
                database.Dispose();
            }
        }
    }
}
