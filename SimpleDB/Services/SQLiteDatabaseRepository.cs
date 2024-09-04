using Chirp.Core.Interfaces;
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
        public Task<IAsyncEnumerable<T>> ReadAsync(int? count)
        {
            throw new NotImplementedException();
        }

        public Task StoreAsync(T record)
        {
            throw new NotImplementedException();
        }
    }
}
