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
using System.Reflection;

namespace SimpleDB.Services
{
    public class CSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        public IEnumerable<T> Read(int? count)
        {
            using (var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data/chirp_cli_db.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                return csv.GetRecords<T>().ToList<T>();
        }

        public void Store(T record)
        {
            using (var writer = new StreamWriter(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data/chirp_cli_db.csv")))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                csv.WriteRecord<T>(record);
        }
    }
}
