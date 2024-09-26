using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chirp.Core.Classes;

namespace Chirp.Core.Helpers
{
    public class DBFacade : IDisposable
    {
        private const string _sqlDBFilePath = "../Chirp.Core/Assets/chirp.db";
        private SqliteConnection? SQLite;
        public DBFacade()
        {
            var fullPath = Path.GetFullPath(_sqlDBFilePath);
            Console.WriteLine($"Database full path: {fullPath}");

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Database file not found at {fullPath}");
            }
            SQLite = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            SQLite.Open();
        }

        public void Dispose()
        {
            if (SQLite != null)
            {
                SQLite.Close();
                SQLite.Dispose();
                SQLite = null;
            }
        }

        public void Insert(string commandText, string author, string message, string timestamp)
        {
            var command = SQLite.CreateCommand();
            command.CommandText = commandText;
            // Adding with parameters to prevent SQL injection
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@timestamp", timestamp);
            command.ExecuteNonQuery();
            Console.WriteLine("The data was inserted into the database.");
        }
        
        public void Delete(string commandText, string author, string message, string timestamp)
        {
            throw new NotImplementedException();
        }
        
        public void Update(string commandText, string author, string message, string timestamp)
        {
            var command = SQLite.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
        
        public List<CheepViewModel> Query(string commandText)
        {
            var cheeps = new List<CheepViewModel>();
            var command = SQLite.CreateCommand();
            command.CommandText = commandText;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var authorValue = reader["username"].ToString();
                var messageValue = reader["text"].ToString();
                double timestampValue = Convert.ToDouble(reader["pub_date"]);
                cheeps.Add(new CheepViewModel(authorValue, messageValue, UnixTimeStampToDateTimeString(timestampValue)));
            }

            return cheeps;
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
