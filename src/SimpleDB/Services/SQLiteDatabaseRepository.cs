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
        IEnumerable<T> IDatabaseRepository<T>.Read(int? count)
        {
            throw new NotImplementedException();
        }

        void IDatabaseRepository<T>.Store(T record)
        {
            throw new NotImplementedException();
        }
    }
}
