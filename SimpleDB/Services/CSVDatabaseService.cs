using SimpleDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDB.Services
{
    public class CSVDatabaseService<T> : IDatabaseRepository<T>
    {
        Task<IEnumerable<T>> IDatabaseRepository<T>.ReadAsync(int? count)
        {
            throw new NotImplementedException();
        }

        Task IDatabaseRepository<T>.StoreAsync(T record)
        {
            throw new NotImplementedException();
        }
    }
}
