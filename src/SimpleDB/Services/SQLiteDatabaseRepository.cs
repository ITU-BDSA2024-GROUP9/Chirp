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
        public Task<List<T>> Read(int? count = null)
        {
            throw new NotImplementedException();
        }

        public Task Store(T record)
        {
            throw new NotImplementedException();
        }
    }
}
