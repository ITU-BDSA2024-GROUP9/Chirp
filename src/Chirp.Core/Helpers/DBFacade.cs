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
        // IMPORTANT!! set environment variable: CHIRPDBPATH as: "../Chirp.Core/Assets/chirp.db"
        // This can be done in the terminal.
        // If this is not set, the database will be created in the temp directory (Which has no test data).
        
        private string _sqlDBFilePath;
        private SqliteConnection? _SQLite;
        public DBFacade()
        {
            _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                             ?? Path.Combine(Path.GetTempPath(), "chirp.db");
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            try
            {
                var fullPath = Path.GetFullPath(_sqlDBFilePath);

                if (!File.Exists(fullPath))
                {
                    CreateDatabase(fullPath);
                }
                _SQLite = new SqliteConnection($"Data Source={_sqlDBFilePath}");
                _SQLite.Open(); 
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(fnfe.Message);
            }

        }
        
        private void CreateDatabase(string fullPath)
        {
            Console.WriteLine($"Creating database at {fullPath}");
            _SQLite = new SqliteConnection($"Data Source={fullPath}");
            _SQLite.Open();

            var createTableQuery = @"
            drop table if exists user;
            create table user (
              user_id integer primary key autoincrement,
              username string not null,
              email string not null,
              pw_hash string not null
            );

            drop table if exists message;
            create table message (
              message_id integer primary key autoincrement,
              author_id integer not null,
              text string not null,
              pub_date integer
            );
            ";

            using var command = new SqliteCommand(createTableQuery, _SQLite);
            command.ExecuteNonQuery();
            _SQLite.Close();
        }

        public void EnsureConnectionInitialized()
        {
            if (_SQLite == null) InitializeConnection();
        }

        public void Dispose()
        {
            EnsureConnectionInitialized();
            
            _SQLite?.Close();
            _SQLite?.Dispose();
            _SQLite = null;
        }
        
        private void ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null)
        {
            try
            {
                EnsureConnectionInitialized();

                var command = _SQLite.CreateCommand();
                command.CommandText = commandText;
                AddParameters(command, parameters);
                command.ExecuteNonQuery();
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
            
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
            try
            {
                EnsureConnectionInitialized();
            
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
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
                return new List<CheepViewModel>();
            }

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
