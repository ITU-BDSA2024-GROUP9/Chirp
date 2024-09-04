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
        async Task<IEnumerable<T>> IDatabaseRepository<T>.ReadAsync(int? count)
        {
            await Task.Run(() => { 
                using (var reader = new StreamReader("SimpleDB/Data/chirp_cli_db.csv"))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        return csv.GetRecords<Cheep>();
                    } 
                }
            });
            throw new Exception();
        }

        async Task IDatabaseRepository<T>.StoreAsync(T record)
        {
            await Task.Run(() => {
                throw new NotImplementedException();
            });
        }
    }
}
