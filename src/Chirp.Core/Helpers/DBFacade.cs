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
        // private const string _sqlDBFilePath = "../Chirp.Core/Assets/chirp.db";
        private const string _sqlDBFilePath = "Assets/chirp.db";
        private SqliteConnection? _SQLite;
        public DBFacade()
        {
            var fullPath = Path.GetFullPath(_sqlDBFilePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Database file not found at {fullPath}");
            }
            _SQLite = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            _SQLite.Open();
        }

        public void Dispose()
        {
            if (_SQLite != null)
            {
                _SQLite.Close();
                _SQLite.Dispose();
                _SQLite = null;
            }
        }

        public void Insert(string commandText, string author, string message, string timestamp)
        {
            var command = _SQLite.CreateCommand();
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
            var command = _SQLite.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
        
        public List<CheepViewModel> Query(string commandText, Dictionary<string, object>? parameters = null)
        {
            var cheeps = new List<CheepViewModel>();
            var command = _SQLite.CreateCommand();
            command.CommandText = commandText;
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var authorValue = reader["username"]?.ToString() ?? "Unknown Author";
                var messageValue = reader["text"]?.ToString() ?? string.Empty;
                var timestampValue = Convert.ToDouble(reader["pub_date"]);
                cheeps.Add(new CheepViewModel(authorValue, messageValue, UnixTimeStampToDateTimeString(timestampValue)));
            }

            return cheeps;
        }
        
        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
        
    }
}