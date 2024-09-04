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
        Task<IEnumerable<T>> IDatabaseRepository<T>.ReadAsync(int? count)
        {
            using (var reader = new StreamReader("SimpleDB/Data/chirp_cli_db.csv"))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Cheep>();
                } 
            }
            
            throw new NotImplementedException();
        }

        Task IDatabaseRepository<T>.StoreAsync(T record)
        {
            throw new NotImplementedException();
        }
    }
}
