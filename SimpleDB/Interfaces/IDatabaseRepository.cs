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
        public IEnumerable<T> Read(int? count = null);
        public void Store(T record);
    }
}
