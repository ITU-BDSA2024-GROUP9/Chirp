using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Helpers
{
    public class DBFacade : IDisposable
    {
        private const string sqlDBFilePath = "/tmp/chirp.db";
        private SqliteConnection SQLite;
        public DBFacade()
        {
            SQLite = new SqliteConnection($"Data Source={sqlDBFilePath}");
            SQLite.Open();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run(string Command)
        {
            //var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";
            var command = SQLite.CreateCommand();
            command.CommandText = Command;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                 
            }
        }
    }
}
