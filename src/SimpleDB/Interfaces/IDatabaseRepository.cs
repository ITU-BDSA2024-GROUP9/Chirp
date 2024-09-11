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
        public Task<List<T>> Read(int? count = null);
        public Task Store(T record);
    }
}
