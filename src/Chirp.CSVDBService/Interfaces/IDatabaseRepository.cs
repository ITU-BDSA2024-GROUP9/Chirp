using Chirp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.CSVDBService.Interfaces
{
    interface IDatabaseRepository<T> where T : IPost
    {
        public Task<IEnumerable<T>> ReadAsync();
        public Task StoreAsync(T record);
    }
}
