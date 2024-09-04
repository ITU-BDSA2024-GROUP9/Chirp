﻿using Chirp.Core.Interfaces;
using SimpleDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDB.Services
{
    public class SQLiteDatabaseRepository<T> : IDatabaseRepository<T> where T : IPost
    {
        Task<IAsyncEnumerable<T>> IDatabaseRepository<T>.ReadAsync(int? count)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseRepository<T>.StoreAsync(T record)
        {
            throw new NotImplementedException();
        }
    }
}
