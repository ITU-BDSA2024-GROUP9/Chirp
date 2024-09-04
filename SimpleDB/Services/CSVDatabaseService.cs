using Chirp.Core.Interfaces;
using SimpleDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Chirp.Core.Classes;

namespace SimpleDB.Services
{
    public class CSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        public async Task<IAsyncEnumerable<T>> ReadAsync(int? count)
        {
            using (var reader = new StreamReader("./Data/chirp_cli_db.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                return csv.GetRecordsAsync<T>();
        }

        public async Task StoreAsync(T record)
        {
            await Task.Run(() => {
                throw new NotImplementedException();
            });
        }
    }
}
