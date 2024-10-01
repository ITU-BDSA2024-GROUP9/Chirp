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
        // "../Chirp.Core/Assets/chirp.db"
        private string _sqlDBFilePath;
        private SqliteConnection? _SQLite;
        public DBFacade()
        {
            Console.WriteLine("Temp path: " + Path.GetFullPath(Path.GetTempPath()));
            _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH")
                             ?? Path.Combine(Path.GetTempPath(), "chirp.db");
            Console.WriteLine("Path: " + _sqlDBFilePath);
            InitializeConnection();
        }

        public void InitializeConnection()
        {
            try
            {
                var fullPath = Path.GetFullPath(_sqlDBFilePath);

                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"Database file not found at {fullPath}");
                }
                _SQLite = new SqliteConnection($"Data Source={_sqlDBFilePath}");
                _SQLite.Open(); 
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(fnfe.Message);
            }

        }

        private void EnsureConnectionInitialized()
        {
            if (_SQLite == null)
            {
                throw new InvalidOperationException("SQLite connection is not initialized.");
            }
        }

        public void Dispose()
        {
            EnsureConnectionInitialized();
            
            _SQLite.Close();
            _SQLite.Dispose();
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
