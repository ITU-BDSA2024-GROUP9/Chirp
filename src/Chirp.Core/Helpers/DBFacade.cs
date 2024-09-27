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
            if (_SQLite == null) return;
            
            _SQLite.Close();
            _SQLite.Dispose();
            _SQLite = null;
        }
        
        public void ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null)
        {
            if (_SQLite == null) return;

            var command = _SQLite.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);
            command.ExecuteNonQuery();
        }

        public void Insert(string commandText, Dictionary<string, object>? parameters = null)
        {
            ExecuteNonQuery(commandText, parameters);
        }
        
        public void Delete(string commandText, Dictionary<string, object>? parameters = null)
        {
            ExecuteNonQuery(commandText, parameters);
        }
        
        public void Update(string commandText, Dictionary<string, object>? parameters = null)
        {
            ExecuteNonQuery(commandText, parameters);
        }
        
        public List<CheepViewModel> Query(string commandText, Dictionary<string, object>? parameters = null)
        {
            if (_SQLite == null) return new List<CheepViewModel>();
            
            var cheeps = new List<CheepViewModel>();
            var command = _SQLite.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);
            
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
        
        private static void AddParameters(SqliteCommand command, Dictionary<string, object>? parameters)
        {
            if (parameters == null) return;
            
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
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
