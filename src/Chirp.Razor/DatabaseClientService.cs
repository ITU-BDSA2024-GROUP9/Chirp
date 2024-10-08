﻿using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SimpleDB.Services
{
    public sealed class DatabaseClientService<T>
    {
        private const string baseURL = "https://bdsagroup9chirprazor.azurewebsites.net";
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
                var response = await client.GetAsync("/cheeps");
                var fullList = await JsonSerializer.DeserializeAsync<List<T>>(await response.Content.ReadAsStreamAsync());

                if (fullList == null)
                {
                    return [];
                }

                if (count == null)
                {
                    return fullList;
                }

                return fullList.TakeLast((int)count).ToList();
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
                await client.PostAsync("/cheep", content);
            }
            finally
            {
                semaphore.Release();
            }
        }

        async public Task Delete(T record) // TODO not supported for now
        {
            await semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(record);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("https://bdsagroup9chirprazor.azurewebsites.net", content);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}