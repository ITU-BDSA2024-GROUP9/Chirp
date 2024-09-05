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
using Microsoft.CSharp.RuntimeBinder;

namespace SimpleDB.Services
{
    public class CSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        private string getFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var projectDirectory = Path.Combine(currentDirectory, "..", "SimpleDB", "Data");
            var test = Path.Combine(projectDirectory, "chirp_cli_db.csv");
            Console.WriteLine(test);
            return test;
        }
        public IEnumerable<T> Read(int? count)
        {
            using (var reader = new StreamReader(getFilePath()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                return csv.GetRecords<T>().ToList<T>();
        }

        public void Store(T record)
        {
            using (var writer = new StreamWriter(getFilePath(), true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.NextRecord(); // Next line in the CSV file
                csv.WriteRecord<T>(record);
            }
                
        }
    }
}