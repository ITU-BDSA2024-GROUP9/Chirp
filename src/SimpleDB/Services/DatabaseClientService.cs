using Chirp.Core.Interfaces;
using SimpleDB.Interfaces;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SimpleDB.Services
{
    public sealed class DatabaseClientService<T> : IDatabaseRepository<T> where T : IPost
    {
        private static string baseURL = "http://localhost:5052";
        private static DatabaseClientService<T>? instance = null;
        private static readonly object padlock = new();
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        private static readonly HttpClient client = new();

        public static DatabaseClientService<T> Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    instance = new DatabaseClientService<T>();
                    client.BaseAddress = new Uri(baseURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                    return instance;
                }

            }
        }

        async public Task<List<T>> Read(int? count = null)
        {
            await semaphore.WaitAsync();
            try
            {
                // first HTTP request
                var requestTask = client.GetAsync("/cheeps");
                var response = await requestTask;
                Console.WriteLine("response: " + response);

                
                await using Stream json = await client.GetStreamAsync("/cheeps");
                var list = await JsonSerializer.DeserializeAsync<List<T>>(json);
                Console.WriteLine("json: " + list.First());
                return list?.Take(count ?? list?.Count ?? 0).ToList() ?? [];
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
                var json = JsonSerializer.Serialize(record);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("localhost:5052", content);
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
                var json = JsonSerializer.Serialize(record);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("localhost:5052", content);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}