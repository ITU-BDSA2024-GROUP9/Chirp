using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Reflection;
using CsvHelper.Configuration;
using Microsoft.CSharp.RuntimeBinder;
using Chirp.Core.Interfaces;

namespace SimpleDB.Services
{
    public sealed class CSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private IWebHostEnvironment env;
        public CSVDatabaseService(IWebHostEnvironment env)
        {
            this.env = env;  
        }
        private string GetFilePath()
        {
            return GetFilePath("chirp_cli_db.csv");
        }

        private string GetFilePath(string filename)
        {
            return Path.Combine(Path.GetTempPath(), env.ContentRootPath, "Data", filename);
        }

        private string GetTMPFilePath()
        {
            return GetTMPFilePath("chirp_cli_db.csv");
        }

        private string GetTMPFilePath(string filename)
        {
            string DataDir = Path.Combine(Path.GetTempPath(), "Data");
            Directory.CreateDirectory(DataDir);
            return Path.Combine(DataDir, filename);
        }

        public async Task<List<T>> Read(int? count = null)
        {
            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetTMPFilePath(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
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

        public async Task Store(T record)
        {
            await semaphore.WaitAsync();
            try
            {
                using var writer = new StreamWriter(new FileStream(GetTMPFilePath(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
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

        public async Task Delete(T record)
        {
            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetTMPFilePath(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite));
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
                using var writer = new StreamWriter(new FileStream(GetTMPFilePath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
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

        async public Task ArrangeTMPDatabase()
        {
            if (File.Exists(GetTMPFilePath())) return;
            await semaphore.WaitAsync();
            try
            {
                using var reader = new StreamReader(new FileStream(GetFilePath("chirp_cli_db_default.csv"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                var toWrite = await reader.ReadToEndAsync();
                reader.Close();
                using var writer = new StreamWriter(new FileStream(GetTMPFilePath(), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
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