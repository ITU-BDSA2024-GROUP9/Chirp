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
using CsvHelper.Configuration;
using Microsoft.CSharp.RuntimeBinder;

namespace SimpleDB.Services
{
    public sealed class CSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        private static CSVDatabaseService<T>? instance = null;
        private static readonly object padlock = new();
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public static CSVDatabaseService<T> Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new CSVDatabaseService<T>();
                    return instance;
                }

            }
        }
        private static string GetFilePath()
        {
            return GetFilePath("chirp_cli_db.csv");
        }

        private static string GetFilePath(string filename)
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projectDirectory = Path.Combine(baseDirectory, "..", "..", "..", "..", "..", "src", "SimpleDB", "Data");
            var filePath = Path.Combine(projectDirectory, filename);
            return Path.GetFullPath(filePath);
        }
        async public Task<List<T>> Read(int? count = null)
        {
            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetFilePath(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecordsAsync<T>();
                var list = new List<T>();
                int i = 0;
                await foreach (var record in records)
                {
                    if (count.HasValue && i++ >= count.Value)
                    {
                        break;
                    }
                    list.Add(record);
                }
                return list;
            }
            finally
            {
                semaphore.Release();
            }
        }

        async public Task Store(T record)
        {
            await semaphore.WaitAsync();
            try
            {
                using var writer = new StreamWriter(new FileStream(GetFilePath(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                using var csv = new CsvWriter(writer, GetConfig());
                await csv.NextRecordAsync(); // Next line in the CSV file
                csv.WriteRecord(record);
                await csv.NextRecordAsync(); // to await the thing
                await csv.FlushAsync(); // also to await the thing
            }
            finally
            {
                semaphore.Release();
            }
        }

        async public Task Delete(T record)
        {
            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetFilePath(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecordsAsync<T>();
                var list = new List<T>();
                await foreach (var r in records)
                {
                    if (!r.Equals(record))
                    {
                        list.Add(r);
                    }
                }
                reader.Close();
                csv.Dispose();
                using var writer = new StreamWriter(new FileStream(GetFilePath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                using var csvWriter = new CsvWriter(writer, GetConfig());
                await csvWriter.WriteRecordsAsync(list);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static CsvConfiguration GetConfig()
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = args => args.Row.Index == 1
            };
        }

        async public Task ArrangeTestDatabase()
        {

            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetFilePath("chirp_cli_db_default.csv"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                var toWrite = await reader.ReadToEndAsync();
                reader.Close();
                using var writer = new StreamWriter(new FileStream(GetFilePath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                await writer.WriteAsync(toWrite);
                writer.Close();
            }
            finally
            {
                semaphore.Release();
            }
        }

    }
}