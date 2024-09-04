using Chirp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDB.Interfaces
{
    interface IDatabaseRepository<T> where T : IPost
    {
        public Task<IEnumerable<T>> ReadAsync(int? count = null);
        public Task StoreAsync(T record);
    }
}
