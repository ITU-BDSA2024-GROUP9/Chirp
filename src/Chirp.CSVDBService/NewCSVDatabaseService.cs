using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Chirp.CSVDBService.Interfaces;
using CsvHelper;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace Chirp.CSVDBService
{
    public class NewCSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        private IWebHostEnvironment env;

        public NewCSVDatabaseService(IWebHostEnvironment env)
        {
            this.env = env;  
        }

        public async Task<IEnumerable<T>> ReadAsync()
        {
            using (var csv = new CsvReader(new StreamReader(Path.Combine(env.ContentRootPath, "Data", "chirp_cli_db.csv")), CultureInfo.InvariantCulture))
            {
                var records = new List<T>();
                await foreach (var record in csv.GetRecordsAsync<T>())
                    records.Add(record);
                return records;
            }
        }

        public async Task StoreAsync(T record)
        {
            using (var csv = new CsvWriter(new StreamWriter(Path.Combine(env.ContentRootPath, "Data", "chirp_cli_db.csv")), CultureInfo.InvariantCulture))
            {
                await csv.NextRecordAsync();
                csv.WriteRecord(record);
                await csv.NextRecordAsync();
                await csv.FlushAsync();
            }
        }
    }
}
